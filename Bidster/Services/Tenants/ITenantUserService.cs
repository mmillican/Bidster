using Bidster.Entities.Tenants;
using Bidster.Entities.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bidster.Services.Tenants
{
    public interface ITenantUserService
    {
        Task<TenantUser> GetByIdAsync(int id);
        Task<TenantUser> GetAsync(int tenantId, int userId);

        Task<IList<TenantUser>> GetByTenantIdAsync(int tenantId);
        Task<IList<TenantUser>> GetByUserIdAsync(int userId);

        Task<IList<User>> GetUsersByTenantIdAsync(int tenantId);
        Task<IList<Tenant>> GetTenantsByUserIdAsync(int userId);

        Task<bool> IsUserInTenantAsync(int userId, int tenantId);

        Task CreateAsync(TenantUser tenantUser);
        Task UpdateAsync(TenantUser tenantUser);
        Task DeleteAsync(TenantUser tenantUser);
    }
}
