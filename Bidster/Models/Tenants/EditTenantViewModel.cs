using Bidster.Entities.Tenants;

namespace Bidster.Models.Tenants
{
    public class EditTenantViewModel : TenantModel
    {
        public string Tab { get; set; }

        public TenantSettings Settings { get; set; }
    }
}
