using Bidster.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Entities.Tenants
{
    public class TenantUser
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public virtual Tenant Tenant { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public DateTime AddedOn { get; set; }

        public bool IsAdmin { get; set; }
    }
}
