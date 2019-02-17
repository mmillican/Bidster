using Bidster.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Models.Tenants
{
    public class TenantUserModel
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public int UserId { get; set; }

        public UserModel User { get; set; }

        public DateTime AddedOn { get; set; }

        public bool IsAdmin { get; set; }
    }
}
