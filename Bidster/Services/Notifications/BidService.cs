using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bidster.Data;
using Bidster.Entities.Bids;
using Bidster.Entities.Events;
using Bidster.Entities.Products;
using Bidster.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bidster.Services.Notifications
{
    public class BidService : IBidService
    {
        private readonly BidsterDbContext _dbContext;
        private readonly ILogger<BidService> _logger;

        public BidService(BidsterDbContext dbContext,
            ILogger<BidService> logger)
        {
            _dbContext = dbContext;
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

            var lastBid = await _dbContext.Bids
                .Where(x => x.ProductId == bidRequest.Product.Id)
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();

            if (bidRequest.Amount < bidRequest.Product.NextMinBidAmount)
            {
                result.ResultType = PlaceBidResultType.InvalidAmount;
                return result;
            }

            try
            {
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
                
                result.ResultType = PlaceBidResultType.Success;
                result.BidId = bid.Id;

                //await __BidNotificationHubContext.Clients.All.SendAsync("SendBidNotification", evt, product);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bid for product ID {id} (User: {userId})", bidRequest.Product.Id, bidRequest.User.Id);

                throw ex;
            }
        }

        public Task SendItemWonNotice(int eventId, int productId)
        {
            throw new NotImplementedException();
        }

        public Task SendOutbidNotice(int eventId, int productId)
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
