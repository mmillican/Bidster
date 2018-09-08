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

        public EventAdminHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EventAdminRequirement requirement, Event resource)
        {
            //context.User.Claims.fi
            var user = await _userManager.GetUserAsync(context.User);
            if (resource.OwnerId == user.Id)
            {
                context.Succeed(requirement);
            }

            var eventAdminIds = resource.Users
                .Where(x => x.EventId == resource.Id && x.IsAdmin)
                .Select(x => x.UserId);
            if (eventAdminIds.Contains(user.Id))
            {
                context.Succeed(requirement);
            }

            context.Fail();
        }
    }
}
