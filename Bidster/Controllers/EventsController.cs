using System;
using System.Linq;
using System.Threading.Tasks;
using Bidster.Data;
using Bidster.Entities.Events;
using Bidster.Entities.Users;
using Bidster.Models.Events;
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
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EventsController> _logger;

        public EventsController(BidsterDbContext dbContext,
            UserManager<User> userManager,
            ILogger<EventsController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            this._logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var events = await _dbContext.Events
                .Select(x => ToEventModel(x))
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
            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == slug);
            if (evt == null)
            {
                _logger.LogInformation("Event not found for slug '{slug}'", slug);
                return RedirectToAction(nameof(Index));
            }

            var model = new EventDetailsViewModel
            {
                Event = ToEventModel(evt)
            };

            return View(model);
        }

        [Authorize]
        [HttpGet("new")]
        public IActionResult Create()
        {
            var model = new EditEventViewModel();

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

            try
            {
                var user = await _userManager.GetUserAsync(User);

                var evt = new Event
                {
                    Name = model.Name,
                    StartOn = model.StartOn,
                    EndOn = model.EndOn,
                    Owner = user,
                    CreatedOn = DateTime.UtcNow
                };

                evt.Slug = await GenerateSlug(evt.Name);

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
            var evt = await _dbContext.Events.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (evt == null || evt.OwnerId != user.Id)
            {
                _logger.LogInformation("Could not find event ID '{id}'", id);
                return RedirectToAction(nameof(Index));
            }

            var model = new EditEventViewModel
            {
                Id = evt.Id,
                Name = evt.Name,
                StartOn = evt.StartOn,
                EndOn = evt.EndOn
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
            
            var evt = await _dbContext.Events.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (evt == null || evt.OwnerId != user.Id)
            {
                _logger.LogInformation("Could not find event ID '{id}'", id);
                return RedirectToAction(nameof(Index));
            }

            try
            {
                evt.Name = model.Name;
                evt.StartOn = model.StartOn;
                evt.EndOn = model.EndOn;

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

        private async Task<string> GenerateSlug(string name)
        {
            var slug = name.Clean();

            if (!await _dbContext.Events.AnyAsync(x => x.Slug == slug))
            {
                return slug;
            }
        
            var appendIdx = 1;
            var modSlug = $"{slug}-{appendIdx})";
            while (await _dbContext.Events.AnyAsync(x => x.Slug == modSlug))
            {
                appendIdx++;
                modSlug = $"{slug}-{appendIdx})";
            }

            return modSlug;
        }

        private static EventModel ToEventModel(Event evt) => new EventModel
        {
            Id = evt.Id,
            Slug = evt.Slug,
            Name = evt.Name,
            StartOn = evt.StartOn,
            EndOn = evt.EndOn,
            OwnerId = evt.OwnerId,
            CreatedOn = evt.CreatedOn
        };
    }
}