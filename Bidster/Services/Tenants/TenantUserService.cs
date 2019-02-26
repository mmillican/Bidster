using Bidster.Data;
using Bidster.Entities.Tenants;
using Bidster.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Services.Tenants
{
    public class TenantUserService : ITenantUserService
    {
        private readonly BidsterDbContext _dbContext;

        public TenantUserService(BidsterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<TenantUser> GetByIdAsync(int id) => _dbContext.TenantUsers.FindAsync(id);

        public Task<TenantUser> GetAsync(int tenantId, int userId) =>
            _dbContext.TenantUsers.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.UserId == userId);

        public async Task<IList<TenantUser>> GetByTenantIdAsync(int tenantId) => 
            await _dbContext.TenantUsers.Include(x => x.User).Where(x => x.TenantId == tenantId).ToListAsync();

        public async Task<IList<TenantUser>> GetByUserIdAsync(int userId) => 
            await _dbContext.TenantUsers.Include(x => x.Tenant).Where(x => x.UserId == userId).ToListAsync();

        public async Task<IList<User>> GetUsersByTenantIdAsync(int tenantId) =>
            await _dbContext.TenantUsers.Include(x => x.User).Where(x => x.TenantId == tenantId).Select(x => x.User).ToListAsync();

        public async Task<IList<Tenant>> GetTenantsByUserIdAsync(int userId) =>
            await _dbContext.TenantUsers.Include(x => x.Tenant).Where(x => x.UserId == userId).Select(x => x.Tenant).ToListAsync();

        public async Task<bool> IsUserInTenantAsync(int userId, int tenantId) =>
            await _dbContext.TenantUsers.AnyAsync(x => x.UserId == userId && x.TenantId == tenantId);

        public async Task CreateAsync(TenantUser tenantUser)
        {
            if (await IsUserInTenantAsync(tenantUser.UserId, tenantUser.TenantId))
            {
                throw new Exception("User is already a member of tenant.");
            }

            _dbContext.TenantUsers.Add(tenantUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TenantUser tenantUser)
        {
            _dbContext.TenantUsers.Update(tenantUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TenantUser tenantUser)
        {
            _dbContext.TenantUsers.Remove(tenantUser);
            await _dbContext.SaveChangesAsync();
        }
    }
}
