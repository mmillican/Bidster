using Bidster.Entities.Tenants;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Services.Tenants
{
    public class TenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantService _tenantService;

        private Tenant _cachedTenant;

        public TenantContext(IHttpContextAccessor httpContextAccessor,
            ITenantService tenantService)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantService = tenantService;
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            if (_cachedTenant != null)
            {
                return _cachedTenant;
            }

            var host = _httpContextAccessor.HttpContext?.Request?.Host.Value;

            var tenants = await _tenantService.GetAllAsync();
            var tenant = tenants.FirstOrDefault(x => _tenantService.ContainsHostName(x, host));

            if (tenant == null)
            {
                tenant = tenants.FirstOrDefault();
            }

            _cachedTenant = tenant ?? throw new Exception("Tenant could not be loaded");
            return _cachedTenant;
        }
    }
}
