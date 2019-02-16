using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bidster.Models;
using Bidster.Data;
using Bidster.Models.Home;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bidster.Entities.Users;
using Bidster.Services.Tenants;

namespace Bidster.Controllers
{
    public class HomeController : Controller
    {
        private readonly BidsterDbContext _dbContext;
        private readonly ITenantContext _tenantContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(BidsterDbContext dbContext,
            ITenantContext tenantContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _dbContext = dbContext;
            _tenantContext = tenantContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var tenant = await _tenantContext.GetCurrentTenantAsync();

            var model = new HomeViewModel();
            model.Settings = await _tenantContext.GetCurrentSettingsAsync();

            model.Events = await _dbContext.Events
                .Where(x => x.TenantId == tenant.Id
                    && x.DisplayOn <= DateTime.Now 
                    && x.EndOn >= DateTime.Now)
                .Select(x => ModelMapper.ToEventModel(x))
                .ToListAsync();

            var eventIds = model.Events.Select(x => x.Id);

            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);

                var userBiddingProductIds = await _dbContext.Bids.Include(x => x.Product)
                    .Where(x => x.Product.TenantId == tenant.Id
                        && eventIds.Contains(x.Product.EventId)
                        && x.UserId == user.Id)
                    .Select(x => x.ProductId)
                    .ToListAsync();

                if (userBiddingProductIds.Any())
                {
                    model.UserBids = await (from bid in _dbContext.Bids.Include(x => x.Product)
                                            where userBiddingProductIds.Contains(bid.ProductId)
                                            group bid by bid.Product into grp
                                            let maxBid = grp.Max(x => x.Amount)
                                            select new BidSummaryModel
                                            {
                                                EventId = grp.Key.EventId,
                                                EventName = model.Events.SingleOrDefault(x => x.Id == grp.Key.EventId).Name,
                                                EventSlug = model.Events.SingleOrDefault(x => x.Id == grp.Key.EventId).Slug,
                                                EventEndTime = model.Events.SingleOrDefault(x => x.Id == grp.Key.EventId).EndOn,
                                                ProductId = grp.Key.Id,
                                                ProductName = grp.Key.Name,
                                                ProductSlug = grp.Key.Slug,
                                                CurrentBidAmount = maxBid,
                                                HighBigUserId = grp.First(x => x.Amount == maxBid).UserId,
                                                IsWinning = grp.First(x => x.Amount == maxBid).UserId == user.Id
                                            })
                                            .ToListAsync();
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
