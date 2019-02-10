using Bidster.Entities.Bids;
using Bidster.Entities.Events;
using Bidster.Entities.Products;
using Bidster.Entities.Tenants;
using Bidster.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bidster.Data
{
    public class BidsterDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Bid> Bids { get; set; }

        public BidsterDbContext(DbContextOptions<BidsterDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(b =>
            {
                b.ToTable("Users");

                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(e => e.Roles)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

            builder.Entity<Role>(b => {
                b.ToTable("Roles");

                b.HasMany(x => x.Users)
                    .WithOne()
                    .HasForeignKey(x => x.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(x => x.Claims)
                    .WithOne()
                    .HasForeignKey(x => x.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
