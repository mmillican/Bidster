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
    public class TenantsController : Controller
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
        public async Task<IActionResult> Edit(int id)
        {
            var tenant = await _tenantService.GetByIdAsync(id);
            if (tenant == null)
            {
                // TODO: Show error
                return RedirectToAction(nameof(Index));
            }

            var model = new EditTenantViewModel
            {
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
    }
}