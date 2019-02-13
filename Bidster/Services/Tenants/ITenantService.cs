using Bidster.Entities.Tenants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bidster.Services.Tenants
{
    public interface ITenantService
    {
        Task<Tenant> GetByIdAsync(int id);
        Task<Tenant> GetByHostAsync(string slug);

        Task<List<Tenant>> GetAllAsync();

        Task<bool> DoesHostNameExistAsync(string hostName, int? existingId = null);

        Task CreateAsync(Tenant tenant);
        Task UpdateAsync(Tenant tenant);

        bool ContainsHostName(Tenant tenant, string host);
    }
}
