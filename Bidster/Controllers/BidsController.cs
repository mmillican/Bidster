using System;
using System.Linq;
using System.Threading.Tasks;
using Bidster.Data;
using Bidster.Entities.Bids;
using Bidster.Entities.Users;
using Bidster.Hubs;
using Bidster.Models.Bids;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bidster.Controllers
{
    [Authorize]
    [Route("bids")]
    public class BidsController : BaseController
    {
        private readonly BidsterDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<BidNotificationHub> __BidNotificationHubContext;
        private readonly ILogger<BidsController> _logger;

        public BidsController(BidsterDbContext dbContext,
            UserManager<User> userManager,
            IHubContext<BidNotificationHub> _bidNotificationHubContext,
            ILogger<BidsController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            __BidNotificationHubContext = _bidNotificationHubContext;
            _logger = logger;
        }

        [HttpPost("")]
        public async Task<IActionResult> Create(BidModel model)
        {
            // TODO: Validations
            // - Make sure user can bid
            // - Make sure last bid wasn't current user

            var product = await _dbContext.Products.FindAsync(model.ProductId);
            var evt = await _dbContext.Events.FindAsync(product.EventId);
            var user = await _userManager.GetUserAsync(User);

            if (product == null)
            {
                _logger.LogInformation("Product ID {id} not found.", model.ProductId);
                return NotFound($"Product ID {model.ProductId} not found");
            }

            if (!evt.IsBiddingOpen(DateTime.Now))
            {
                return BadRequest("Bidding is not open");
            }

            var lastBid = await _dbContext.Bids
                .Where(x => x.ProductId == product.Id)
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();

            if (model.Amount < product.NextMinBidAmount)
            {
                AddErrorNotice($"Bid must be {product.NextMinBidAmount.ToString("c2")} or greater");
                return RedirectToProduct(evt.Slug, product.Slug);
            }

            try
            {
                var bid = new Bid
                {
                    Product = product,
                    User = user,
                    Timestamp = DateTime.Now,
                    Amount = model.Amount
                };

                product.CurrentBidAmount = bid.Amount;
                product.CurrentHighBidUserId = user.Id;
                product.BidCount++;
                _dbContext.Products.Update(product);

                _dbContext.Bids.Add(bid);
                await _dbContext.SaveChangesAsync();

                AddSuccessNotice("Your bid has been placed!");

                await __BidNotificationHubContext.Clients.All.SendAsync("SendBidNotification", evt, product);
                
                return RedirectToProduct(evt.Slug, product.Slug);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error placing bid for product ID {id} (User: {userId})", model.ProductId, user.Id);
                return StatusCode(500); // TODO: Return something different
            }
        }

        private ActionResult RedirectToProduct(string evtSlug, string slug) =>
            RedirectToAction("Details", "Products", new { evtSlug = evtSlug, slug = slug });        
    }
}