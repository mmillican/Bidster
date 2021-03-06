﻿using Bidster.Data;
using Bidster.Entities.Users;
using Bidster.Models;
using Bidster.Models.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Controllers
{
    public class HomeController : Controller
    {
        private readonly BidsterDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(BidsterDbContext dbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {

            var model = new HomeViewModel();

            model.Events = await _dbContext.Events
                .Where(x => x.DisplayOn <= DateTime.Now 
                    && x.EndOn >= DateTime.Now)
                .Select(x => ModelMapper.ToEventModel(x))
                .ToListAsync();

            var eventIds = model.Events.Select(x => x.Id);

            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);

                var userBiddingProductIds = await _dbContext.Bids.Include(x => x.Product)
                    .Where(x => eventIds.Contains(x.Product.EventId)
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
