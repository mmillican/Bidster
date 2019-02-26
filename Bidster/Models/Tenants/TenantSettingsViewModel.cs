using Bidster.Entities.Tenants;

namespace Bidster.Models.Tenants
{
    public class TenantSettingsViewModel
    {
        public int Id { get; set; }

        public TenantSettings Settings { get; set; }
    }
}
