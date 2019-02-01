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

namespace Bidster.Controllers
{
    public class HomeController : Controller
    {
        private readonly BidsterDbContext _dbContext;

        public HomeController(BidsterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel();
            model.Events = await _dbContext.Events
                .Where(x => x.DisplayOn <= DateTime.Now 
                    && x.EndOn >= DateTime.Now)
                .Select(x => ModelMapper.ToEventModel(x))
                .ToListAsync();

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
