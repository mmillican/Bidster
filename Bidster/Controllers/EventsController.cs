using System;
using System.Linq;
using System.Threading.Tasks;
using Bidster.Data;
using Bidster.Entities.Events;
using Bidster.Entities.Tenants;
using Bidster.Entities.Users;
using Bidster.Models;
using Bidster.Models.Events;
using Bidster.Services.FileStorage;
using Bidster.Services.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bidster.Controllers
{
    [Route("events")]
    public class EventsController : Controller
    {
        private readonly BidsterDbContext _dbContext;
        private readonly ITenantContext _tenantContext;
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFileService _fileService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(BidsterDbContext dbContext,
            ITenantContext tenantContext,
            UserManager<User> userManager,
            IAuthorizationService authorizationService,
            IFileService fileService,
            ILogger<EventsController> logger)
        {
            _dbContext = dbContext;
            _tenantContext = tenantContext;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _fileService = fileService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var tenant = await _tenantContext.GetCurrentTenantAsync();

            var events = await _dbContext.Events
                .Where(x => x.TenantId == tenant.Id)
                .Select(x => ModelMapper.ToEventModel(x))
                .ToListAsync();

            var model = new EventListViewModel
            {
                Events = events
            };

            return View(model);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var tenant = await _tenantContext.GetCurrentTenantAsync();

            var evt = await _dbContext.Events.Include(x => x.Users).SingleOrDefaultAsync(x => x.Slug == slug);
            if (evt == null || evt.TenantId != tenant.Id)
            {
                _logger.LogInformation("Event not found for slug '{slug}'", slug);
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);

            var model = new EventDetailsViewModel
            {
                Event = ModelMapper.ToEventModel(evt)
            };
            model.CanUserEdit = await AuthorizeEventAdmin(evt);

            model.Products = await _dbContext.Products.Where(x => x.EventId == evt.Id)
                .Select(x => ModelMapper.ToProductModel(x))
                .ToListAsync();

            foreach (var product in model.Products)
            {
                if (!string.IsNullOrEmpty(product.ThumbnailFilename))
                {
                    product.ThumbnailUrl = _fileService.ResolveFileUrl(string.Format(Constants.ImagePathFormat, evt.Slug, product.ThumbnailFilename));
                }
            }

            return View(model);
        }

        [Authorize]
        [HttpGet("{slug}/all-bids")]
        public async Task<IActionResult> AllBids(string slug)
        {
            var tenant = await _tenantContext.GetCurrentTenantAsync();

            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == slug);
            if (evt == null || evt.TenantId != tenant.Id)
            {
                _logger.LogInformation("Event not found for slug '{slug}'", slug);
                return RedirectToAction(nameof(Index));
            }

            if (!await AuthorizeEventAdmin(evt))
            {
                return Unauthorized();
            }

            var eventProducts = await _dbContext.Products
                .Where(x => x.EventId == evt.Id)
                .AsNoTracking()
                .ToListAsync();

            var productBidGroups = await (from bid in _dbContext.Bids.Include(x => x.User)
                                          join prod in _dbContext.Products on bid.ProductId equals prod.Id
                                          where prod.EventId == evt.Id
                                          group bid by bid.ProductId into g
                                          select new
                                          {
                                              ProductId = g.Key,
                                              Bids = g.OrderByDescending(x => x.Timestamp)
                                          })
                                    .AsNoTracking()
                                    .ToListAsync();

            var model = new AllBidsReportViewModel
            {
                Event = evt.ToEventModel()
            };

            foreach (var pbg in productBidGroups)
            {
                var product = eventProducts.SingleOrDefault(x => x.Id == pbg.ProductId);
                if (product == null)
                {
                    continue;
                }

                var pgm = new AllBidsReportViewModel.ProductGroupModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Bids = pbg.Bids.Select(x => x.ToBidModel()).ToList()
                };
                model.Products.Add(pgm);
            }

            return View(model);
        }

        [Authorize]
        [HttpGet("{slug}/winning-bids")]
        public async Task<IActionResult> WinningBids(string slug)
        {
            var tenant = await _tenantContext.GetCurrentTenantAsync();

            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == slug);
            if (evt == null || evt.TenantId != tenant.Id)
            {
                _logger.LogInformation("Event not found for slug '{slug}'", slug);
                return RedirectToAction(nameof(Index));
            }

            if (!await AuthorizeEventAdmin(evt))
            {
                return Unauthorized();
            }

            var winningBids = await (from bid in _dbContext.Bids
                                        .Include(x => x.User)
                                        .Include(x => x.Product)
                                     where bid.Product.EventId == evt.Id
                                     group bid by bid.Product into grp
                                     select new
                                     {
                                         Product = grp.Key,
                                         WinningBid = grp.OrderByDescending(x => x.Amount).FirstOrDefault()
                                     })
                                     .AsNoTracking()
                                     .ToListAsync();

            var model = new WinningBidsReportViewModel
            {
                Event = evt.ToEventModel()
            };

            foreach (var winBid in winningBids)
            {
                var bidModel = new WinningBidsReportViewModel.BidRecordModel
                {
                    Id = winBid.WinningBid.Id,
                    ProductId = winBid.Product.Id,
                    ProductName = winBid.Product.Name,
                    BidTimestamp = winBid.WinningBid.Timestamp,
                    BidAmount = winBid.WinningBid.Amount,
                    Winner = new WinningBidsReportViewModel.BidRecordModel.UserModel
                    {
                        Id = winBid.WinningBid.User.Id,
                        FullName = winBid.WinningBid.User.FullName,
                        Address = winBid.WinningBid.User.Address,
                        Address2 = winBid.WinningBid.User.Address2,
                        City = winBid.WinningBid.User.City,
                        State = winBid.WinningBid.User.State,
                        PostalCode = winBid.WinningBid.User.PostalCode,
                        PhoneNumber = winBid.WinningBid.User.PhoneNumber
                    }
                };
                model.WinningBids.Add(bidModel);
            }

            return View(model);
        }

        [Authorize]
        [HttpGet("new")]
        public IActionResult Create()
        {
            var model = new EditEventViewModel();
            model.StartOn = DateTime.Now.Date;
            model.EndOn = DateTime.Now.Date.AddDays(1);
            model.DisplayOn = DateTime.Now.Date;

            return View(model);
        }

        [Authorize]
        [HttpPost("new")]
        public async Task<IActionResult> Create(EditEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tenant = await _tenantContext.GetCurrentTenantAsync();

            try
            {
                var user = await _userManager.GetUserAsync(User);

                var evt = new Event
                {
                    Tenant = tenant,
                    Name = model.Name,
                    Description = model.Description,
                    StartOn = model.StartOn,
                    EndOn = model.EndOn,
                    DisplayOn = model.DisplayOn,
                    HideBidderNames = model.HideBidderNames,
                    DefaultMinimumBidAmount = model.DefaultMinimumBidAmount,
                    Owner = user,
                    CreatedOn = DateTime.UtcNow
                };

                evt.Slug = await GenerateSlug(tenant, evt.Name);

                _dbContext.Events.Add(evt);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error creating event '{name}'", model.Name);
                return View(model);
            }
        }

        [Authorize]
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var tenant = await _tenantContext.GetCurrentTenantAsync();

            var evt = await _dbContext.Events.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (evt == null || evt.TenantId != tenant.Id)
            {
                _logger.LogInformation("Could not find event ID '{id}'", id);
                return RedirectToAction(nameof(Index));
            }

            if (!await AuthorizeEventAdmin(evt))
            {
                return Unauthorized();
            }

            var model = new EditEventViewModel
            {
                Id = evt.Id,
                Name = evt.Name,
                Description = evt.Description,
                StartOn = evt.StartOn,
                EndOn = evt.EndOn,
                DisplayOn = evt.DisplayOn,
                HideBidderNames = evt.HideBidderNames,
                DefaultMinimumBidAmount = evt.DefaultMinimumBidAmount,
            };

            return View(model);
        }

        [Authorize]
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, EditEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tenant = await _tenantContext.GetCurrentTenantAsync();

            var evt = await _dbContext.Events.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (evt == null || evt.TenantId != tenant.Id)
            {
                _logger.LogInformation("Could not find event ID '{id}'", id);
                return RedirectToAction(nameof(Index));
            }

            if (!await AuthorizeEventAdmin(evt))
            {
                return Unauthorized();
            }

            try
            {
                evt.Name = model.Name;
                evt.Description = model.Description;
                evt.StartOn = model.StartOn;
                evt.EndOn = model.EndOn;
                evt.DisplayOn = model.DisplayOn;
                evt.HideBidderNames = model.HideBidderNames;
                evt.DefaultMinimumBidAmount = model.DefaultMinimumBidAmount;

                _dbContext.Events.Update(evt);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { slug = evt.Slug });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating event ID '{id}'", id);
                return View(model);
            }
        }

        private async Task<string> GenerateSlug(Tenant tenant, string name)
        {
            var slug = name.Clean();

            if (!await _dbContext.Events.AnyAsync(x => x.TenantId == tenant.Id && x.Slug == slug))
            {
                return slug;
            }
        
            var appendIdx = 1;
            var modSlug = $"{slug}-{appendIdx})";
            while (await _dbContext.Events.AnyAsync(x => x.TenantId == tenant.Id && x.Slug == modSlug))
            {
                appendIdx++;
                modSlug = $"{slug}-{appendIdx})";
            }

            return modSlug;
        }

        // TODO: Would be nice to have this be an actual auth policy, but need to figure out adding claims first
        private async Task<bool> AuthorizeEventAdmin(Event evt)
        {
            // TODO: Needs to be tenant aware 
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return false;
            }

            if (user.Id == evt.OwnerId)
            {
                return true;
            }

            var isEventAdmin = await _dbContext.EventUsers
                .AnyAsync(x => x.EventId == evt.Id
                    && x.UserId == user.Id
                    && x.IsAdmin);

            return isEventAdmin;
        }
    }
}