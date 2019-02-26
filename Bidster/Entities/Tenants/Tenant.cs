using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bidster.Entities.Tenants
{
    public class Tenant
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string HostNames { get; set; }

        public bool IsDisabled { get; set; }

        public string Settings { get; set; }

        public virtual ICollection<TenantUser> Users { get; } = new List<TenantUser>();
    }
}
