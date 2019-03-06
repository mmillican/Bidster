using Bidster.Entities.Tenants;
using Bidster.Entities.Users;
using Bidster.Services.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Bidster.Auth
{
    public class TenantAdminRequirement : IAuthorizationRequirement
    {
    }

    public class TenantAdminHandler : AuthorizationHandler<TenantAdminRequirement, Tenant>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITenantUserService _tenantUserService;

        public TenantAdminHandler(UserManager<User> userManager,
            ITenantUserService tenantUserService)
        {
            _userManager = userManager;
            _tenantUserService = tenantUserService;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantAdminRequirement requirement, Tenant resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                context.Fail();
                return;
            }

            var tenantUser = await _tenantUserService.GetAsync(resource.Id, user.Id);
            if (tenantUser?.IsAdmin ?? false)
            {
                context.Succeed(requirement);
            }            
        }
    }
}
