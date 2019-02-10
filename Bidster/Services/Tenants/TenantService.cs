using Bidster.Data;
using Bidster.Entities.Tenants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Services.Tenants
{
    public class TenantService : ITenantService
    {
        private readonly BidsterDbContext _dbContext;

        public TenantService(BidsterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Tenant> GetByIdAsync(int id) => _dbContext.Tenants.FindAsync(id);
        public Task<Tenant> GetByHostAsync(string slug) => _dbContext.Tenants.SingleOrDefaultAsync(x => x.HostNames == slug);

        public Task<List<Tenant>> GetAllAsync() => _dbContext.Tenants.ToListAsync();

        public async Task CreateAsync(Tenant tenant)
        {
            _dbContext.Tenants.Add(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
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
