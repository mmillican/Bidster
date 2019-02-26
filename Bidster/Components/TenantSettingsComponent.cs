using Bidster.Models.Tenants;
using Bidster.Services.Tenants;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bidster.Components
{
    [ViewComponent(Name = "TenantSettings")]
    public class TenantSettingsComponent : ViewComponent
    {
        private readonly ITenantService _tenantService;

        public TenantSettingsComponent(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int tenantId)
        {
            var model = new TenantSettingsViewModel
            {
                Id = tenantId
            };

            model.Settings = await _tenantService.GetSettingsAsync(tenantId);

            return View(model);
        }
    }
}
