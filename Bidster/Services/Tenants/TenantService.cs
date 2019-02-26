using Bidster.Data;
using Bidster.Entities.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Services.Tenants
{
    public class TenantService : ITenantService
    {
        private readonly BidsterDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public TenantService(BidsterDbContext dbContext,
            IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Tenant> GetByIdAsync(int id)
        {
            var cacheKey = string.Format(CacheKeys.TenantById, id);

            Tenant tenant;
            if (!_cache.TryGetValue(cacheKey, out tenant))
            {
                tenant = await _dbContext.Tenants.FindAsync(id);
                if (tenant == null)
                {
                    return null;
                }

                _cache.Set(cacheKey, tenant, DateTime.Now.AddMinutes(10));
            }
            
            return tenant;
        }

        public Task<Tenant> GetByHostAsync(string slug) => _dbContext.Tenants.SingleOrDefaultAsync(x => x.HostNames == slug);

        public Task<List<Tenant>> GetAllAsync() => _dbContext.Tenants.ToListAsync();

        public Task<bool> DoesHostNameExistAsync(string hostName, int? existingId = null) =>
            _dbContext.Tenants.AnyAsync(x => (!existingId.HasValue || x.Id != existingId.Value) && x.HostNames.Contains(hostName));

        public async Task CreateAsync(Tenant tenant)
        {
            _dbContext.Tenants.Add(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
            
            _cache.Remove(string.Format(CacheKeys.TenantById, tenant.Id));
            _cache.Remove(string.Format(CacheKeys.TenantByHost, tenant.HostNames));
            _cache.Remove(string.Format(CacheKeys.TenantSettings, tenant.Id));
        }

        public async Task<TenantSettings> GetSettingsAsync(int tenantId)
        {
            var cacheKey = string.Format(CacheKeys.TenantSettings, tenantId);

            TenantSettings settings;
            if (!_cache.TryGetValue(cacheKey, out settings))
            {
                var tenant = await GetByIdAsync(tenantId);
                if (tenant == null || string.IsNullOrEmpty(tenant.Settings))
                {
                    return null;
                }

                settings = JsonConvert.DeserializeObject<TenantSettings>(tenant.Settings);

                _cache.Set(cacheKey, settings, TimeSpan.FromMinutes(10));
            }

            return settings;
        }

        public async Task SaveSettingsAsync(int tenantId, TenantSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var tenant = await GetByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new Exception("Tenant not found");
            }

            tenant.Settings = JsonConvert.SerializeObject(settings);
            await UpdateAsync(tenant);            
        }

        public string[] ParseTenantHosts(Tenant tenant)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            
            if (string.IsNullOrEmpty(tenant.HostNames))
            {
                return new string[0];
            }

            var parsedHosts = tenant.HostNames.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return parsedHosts;
        }

        public bool ContainsHostName(Tenant tenant, string host)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            if (string.IsNullOrEmpty(host))
            {
                return false;
            }

            var containsHost = ParseTenantHosts(tenant).Any(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase));
            return containsHost;
        }
    }
}
