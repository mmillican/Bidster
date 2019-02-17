using System;
using System.Linq;
using System.Threading.Tasks;
using Bidster.Data;
using Bidster.Entities.Events;
using Bidster.Entities.Users;
using Bidster.Models;
using Bidster.Models.EventUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bidster.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("events/{evtSlug}/users")]
    public class EventUsersController : Controller
    {
        private readonly BidsterDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EventUsersController> _logger;

        public EventUsersController(BidsterDbContext dbContext,
            UserManager<User> userManager,
            ILogger<EventUsersController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string evtSlug)
        {
            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == evtSlug);
            if (evt == null)
            {
                _logger.LogInformation("Event with slug '{slug}' not found", evtSlug);
                return NotFound();
            }

            var evtUsers = await _dbContext.EventUsers.Include(x => x.User)
                .Where(x => x.EventId == evt.Id)
                .OrderBy(x => x.User.FullName)
                .Select(x => x.ToEventUserModel())
                .ToListAsync();

            var model = new EventUserListViewModel
            {
                Event = evt.ToEventModel(),
                Users = evtUsers
            };

            return View("Index", model);
        }

        [HttpPost("new")]
        public async Task<IActionResult> Create(string evtSlug, EventUserListViewModel model)
        {
            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == evtSlug);
            if (evt == null)
            {
                _logger.LogInformation("Event with slug '{slug}' not found", evtSlug);
                return NotFound();
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.NewUserEmail);
                if (user == null)
                {
                    ModelState.AddModelError("NewUserEmail", "User with this email does not exist.");
                    return await Index(evtSlug);
                }

                var existingUser = await _dbContext.EventUsers.SingleOrDefaultAsync(x => x.EventId == evt.Id && x.UserId == user.Id);
                if (existingUser != null)
                {
                    ModelState.AddModelError("NewUserEmail", "User is alredy a member of this event.");
                    return await Index(evtSlug);
                }

                var evtUser = new EventUser
                {
                    Event = evt,
                    User = user,
                    IsAdmin = model.NewUserIsAdmin,
                    CreatedOn = DateTime.Now
                };

                _dbContext.EventUsers.Add(evtUser);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { slug = evtSlug });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error adding user '{user}' to event {id}", model.NewUserEmail, evt.Id);

                ModelState.AddModelError("", "Error adding user to event");
                return await Index(evtSlug);
            }

        }

        [HttpGet("delete")]
        public async Task<IActionResult> Delete(string evtSlug, int id)
        {
            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == evtSlug);
            if (evt == null)
            {
                _logger.LogInformation("Event with slug '{slug}' not found", evtSlug);
                return NotFound();
            }

            //try
            {
                var existingUser = await _dbContext.EventUsers.SingleOrDefaultAsync(x => x.EventId == evt.Id && x.Id == id);
                if (existingUser != null)
                {
                    _dbContext.EventUsers.Remove(existingUser);
                    await _dbContext.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index), new { evtSlug });

            }
        }
    }
}
