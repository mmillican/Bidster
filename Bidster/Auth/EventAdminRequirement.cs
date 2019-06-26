using System.Linq;
using System.Threading.Tasks;
using Bidster.Data;
using Bidster.Entities.Events;
using Bidster.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bidster.Auth
{
    public class EventAdminRequirement : IAuthorizationRequirement
    {
    }

    public class EventAdminHandler : AuthorizationHandler<EventAdminRequirement, Event>
    {
        private readonly UserManager<User> _userManager;
        private readonly BidsterDbContext _dbContext;

        public EventAdminHandler(UserManager<User> userManager,
            BidsterDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EventAdminRequirement requirement, Event resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                context.Fail();
                return;
            }

            if (resource.OwnerId == user.Id)
            {
                context.Succeed(requirement);
                return;
            }

            var eventAdminIds = await _dbContext.EventUsers
                .Where(x => x.EventId == resource.Id && x.IsAdmin)
                .Select(x => x.UserId)
                .ToListAsync();

            if (eventAdminIds.Contains(user.Id))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
