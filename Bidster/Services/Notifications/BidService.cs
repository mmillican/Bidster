using System;
using System.Linq;
using System.Threading.Tasks;
using Bidster.Data;
using Bidster.Entities.Bids;
using Bidster.Entities.Events;
using Bidster.Entities.Products;
using Bidster.Entities.Users;
using Bidster.Models.EmailTemplates;
using Bidster.Services.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bidster.Services.Notifications
{
    public class BidService : IBidService
    {
        private readonly BidsterDbContext _dbContext;
        private readonly IViewRenderer _viewRenderer;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<BidService> _logger;

        public BidService(BidsterDbContext dbContext,
            IViewRenderer viewRenderer,
            IEmailSender emailSender,
            ILogger<BidService> logger)
        {
            _dbContext = dbContext;
            _viewRenderer = viewRenderer;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<PlaceBidResult> PlaceBidAsync(PlaceBidRequest bidRequest)
        {
            var result = new PlaceBidResult();

            if (!bidRequest.Event.IsBiddingOpen(DateTime.Now))
            {
                result.ResultType = PlaceBidResultType.BiddingClosed;
                return result;
            }
            
            if (bidRequest.Amount < bidRequest.Product.NextMinBidAmount)
            {
                result.ResultType = PlaceBidResultType.InvalidAmount;
                return result;
            }

            try
            {
                var previousBid = await GetLatestBidAsync(bidRequest.Product.Id);

                var bid = new Bid
                {
                    Product = bidRequest.Product,
                    User = bidRequest.User,
                    Timestamp = DateTime.Now,
                    Amount = bidRequest.Amount
                };

                _dbContext.Bids.Add(bid);
                await _dbContext.SaveChangesAsync();

                bidRequest.Product.CurrentBidAmount = bid.Amount;
                bidRequest.Product.CurrentHighBidUserId = bidRequest.User.Id;
                bidRequest.Product.BidCount++;

                _dbContext.Products.Update(bidRequest.Product);
                await _dbContext.SaveChangesAsync();
                
                result.ResultType = PlaceBidResultType.Success;
                result.BidId = bid.Id;

                //await __BidNotificationHubContext.Clients.All.SendAsync("SendBidNotification", evt, product);

                if (previousBid != null)
                {
                    await SendOutbidNoticeAsync(bidRequest.Event, bidRequest.Product, previousBid, bid);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bid for product ID {id} (User: {userId})", bidRequest.Product.Id, bidRequest.User.Id);

                throw ex;
            }
        }

        private async Task<Bid> GetLatestBidAsync(int productId)
        {
            var bid = await _dbContext.Bids.AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();
            return bid;
        }

        private async Task SendOutbidNoticeAsync(Event evt, Product product, Bid previousBid, Bid highBid)
        {
            try
            {
                var subject = $"[{evt.Name}] You've been outbid on {product.Name}";
                var messageModel = new OutbidNoticeModel
                {
                    EventId = evt.Id,
                    EventName = evt.Name,
                    EventEndDate = evt.EndOn,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UserName = previousBid.User.FullName,
                    OutbidTime = highBid.Timestamp,
                    OutbidAmount = previousBid.Amount,
                    NewHighBidAmount = highBid.Amount
                };

                var messageBody = await _viewRenderer.RenderViewToStringAsync("EmailTemplates/OutbidNotice", messageModel);

                await _emailSender.SendEmailAsync(previousBid.User.Email, subject, messageBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending outbid notice for product ID {productId}", previousBid.ProductId);
            }
        }

        public Task SendItemWonNoticeAsync(int eventId, int productId)
        {
            throw new NotImplementedException();
        }

    }

    public class PlaceBidRequest
    {
        public Event Event { get; set; }
        public Product Product { get; set; }
        public User User { get; set; }

        public decimal Amount { get; set; }
    }

    public class PlaceBidResult
    {
        public PlaceBidResultType ResultType { get; set; }

        public int? BidId { get; set; }
    }

    public enum PlaceBidResultType
    {
        Success = 0,
        BiddingClosed = 1,
        InvalidAmount = 2,
    }
}
