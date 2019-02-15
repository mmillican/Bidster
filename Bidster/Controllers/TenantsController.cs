using Bidster.Entities.Tenants;
using Bidster.Models;
using Bidster.Models.Tenants;
using Bidster.Services.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Controllers
{
    [Authorize] // TODO: Super admin only
    [Route("tenants")]
    public class TenantsController : BaseController
    {
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(ITenantService tenantService,
            ILogger<TenantsController> logger)
        {
            _tenantService = tenantService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var model = new TenantListViewModel();
            model.Tenants = (await _tenantService.GetAllAsync()).Select(x => x.ToTenantModel()).ToList();

            return View(model);
        }

        [HttpGet("new")]
        public IActionResult Create() => View(new EditTenantViewModel());

        [HttpPost("new")]
        public async Task<IActionResult> Create(EditTenantViewModel model)
        {
            if (!string.IsNullOrEmpty(model.HostNames))
            {
                var hostNames = model.HostNames.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach(var hostName in hostNames)
                {
                    if (await _tenantService.DoesHostNameExistAsync(hostName))
                    {
                        ModelState.AddModelError(nameof(model.HostNames), $"Host name '{hostName}' is not available.");
                        break;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                // TODO: show error
                return View(model);
            }

            try
            {
                var tenant = new Tenant
                {
                    Name = model.Name,
                    HostNames = model.HostNames,
                    IsDisabled = model.IsDisabled
                };

                await _tenantService.CreateAsync(tenant);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error creating tenant");
                // TODO: show error
                return View(model);
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id, string tab = null)
        {
            var tenant = await _tenantService.GetByIdAsync(id);
            if (tenant == null)
            {
                // TODO: Show error
                return RedirectToAction(nameof(Index));
            }

            var model = new EditTenantViewModel
            {
                Tab = tab,
                Id = tenant.Id,
                Name = tenant.Name,
                HostNames = tenant.HostNames,
                IsDisabled = tenant.IsDisabled
            };

            return View(model);
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, EditTenantViewModel model)
        {
            var tenant = await _tenantService.GetByIdAsync(id);
            if (tenant == null)
            {
                // TODO: Show error
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(model.HostNames))
            {
                var hostNames = model.HostNames.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var hostName in hostNames)
                {
                    if (await _tenantService.DoesHostNameExistAsync(hostName, model.Id))
                    {
                        ModelState.AddModelError(nameof(model.HostNames), $"Host name '{hostName}' is not available.");
                        break;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                // TODO: show error
                return View(model);
            }

            try
            {
                tenant.Name = model.Name;
                tenant.HostNames = model.HostNames;
                tenant.IsDisabled = model.IsDisabled;

                await _tenantService.UpdateAsync(tenant);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant {id}", id);
                // TODO: show error
                return View(model);
            }

        }

        [HttpPost("settings/{tenantId}")]
        public async Task<IActionResult> UpdateSettings(int tenantId, TenantSettingsViewModel model)
        {
            var tenant = await _tenantService.GetByIdAsync(tenantId);
            if (tenant == null)
            {
                // TODO: check if user has access to tenant
                // TODO: show error
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Not really sure how I feel about exposing the actual TenantSettings to the UI
                // But for purposes of "MVP" it works for now. Revisit later
                await _tenantService.SaveSettingsAsync(tenantId, model.Settings);

                AddSuccessNotice("The settings have been updated.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error saving settings for tenant {id}", tenantId);

                AddErrorNotice("There was an error updating the settings.");
            }

            return RedirectToAction(nameof(Edit), new { id = tenantId, tab = "settings" }); // TODO: Not ideal because un-saved form values will be lost
        }
    }
}