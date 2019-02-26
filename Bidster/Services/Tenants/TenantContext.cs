using Bidster.Entities.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Services.Tenants
{
    public class TenantContext : ITenantContext
    {
        private const int CACHE_DURATION = 10; // minutes

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantService _tenantService;
        private readonly IMemoryCache _memoryCache;

        public TenantContext(IHttpContextAccessor httpContextAccessor,
            ITenantService tenantService,
            IMemoryCache memoryCache)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantService = tenantService;
            _memoryCache = memoryCache;
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var host = _httpContextAccessor.HttpContext?.Request?.Host.Value;
            var cacheKey = string.Format(CacheKeys.TenantByHost, host);

            Tenant tenant;
            if (!_memoryCache.TryGetValue(cacheKey, out tenant))
            {
                var tenants = await _tenantService.GetAllAsync();
                tenant = tenants.FirstOrDefault(x => _tenantService.ContainsHostName(x, host));

                if (tenant == null)
                {
                    tenant = tenants.FirstOrDefault();
                }

                if (tenant == null)
                {
                    throw new Exception("Tenant could not be loaded.");
                }
                _memoryCache.Set(cacheKey, tenant, DateTime.Now.AddMinutes(CACHE_DURATION));
            };

            return tenant;
        }

        public async Task<TenantSettings> GetCurrentSettingsAsync()
        {
            var tenant = await GetCurrentTenantAsync();
            if (tenant == null)
            {
                throw new Exception("Could not retrieve settings because tenant could not be loaded.");
            }

            var settings = await _tenantService.GetSettingsAsync(tenant.Id);
            return settings;
        }
    }
}
