using System;
using System.Linq;
using System.Threading.Tasks;
using Bidster.Data;
using Bidster.Entities.Bids;
using Bidster.Entities.Users;
using Bidster.Hubs;
using Bidster.Models.Bids;
using Bidster.Services.Notifications;
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
        private readonly IBidService _bidService;
        private readonly ILogger<BidsController> _logger;

        public BidsController(BidsterDbContext dbContext,
            UserManager<User> userManager,
            IBidService bidService,
            ILogger<BidsController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _bidService = bidService;
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

            var placeBidRequest = new PlaceBidRequest
            {
                Event = evt,
                Product = product,
                User = user,
                Amount = model.Amount
            };

            try
            {
                var bidResult = await _bidService.PlaceBidAsync(placeBidRequest);
                
                if (bidResult.ResultType == PlaceBidResultType.BiddingClosed)
                {
                    return BadRequest("Bidding for event is not open");
                }
                else if (bidResult.ResultType == PlaceBidResultType.InvalidAmount)
                {
                    return BadRequest($"Bid amount must be {product.NextMinBidAmount} or greater");
                }

                // Success
                return RedirectToProduct(evt.Slug, product.Slug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bid for product ID {id} (User: {userId})", model.ProductId, user.Id);
                return StatusCode(500); // TODO: Return something different
            }
        }

        private ActionResult RedirectToProduct(string evtSlug, string slug) =>
            RedirectToAction("Details", "Products", new { evtSlug = evtSlug, slug = slug });        
    }
}