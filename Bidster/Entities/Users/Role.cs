using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Bidster.Entities.Users
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<IdentityUserRole<int>> Users { get; } = new List<IdentityUserRole<int>>();

        public virtual ICollection<IdentityRoleClaim<int>> Claims { get; } = new List<IdentityRoleClaim<int>>();
    }
}
