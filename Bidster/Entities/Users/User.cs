using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bidster.Entities.Tenants;
using Microsoft.AspNetCore.Identity;

namespace Bidster.Entities.Users
{
    public class User : IdentityUser<int>
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [MaxLength(50)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string Address2 { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(5)]
        public string State { get; set; }
        [MaxLength(15)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<int>> Roles { get; } = new List<IdentityUserRole<int>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<int>> Claims { get; } = new List<IdentityUserClaim<int>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<int>> Logins { get; } = new List<IdentityUserLogin<int>>();

        public virtual ICollection<TenantUser> Tenants { get; } = new List<TenantUser>();
    }
}
