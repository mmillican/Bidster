using Bidster.Entities.Tenants;
using System.Threading.Tasks;

namespace Bidster.Services.Tenants
{
    public interface ITenantContext
    {
        Task<Tenant> GetCurrentTenantAsync();
    }
}
