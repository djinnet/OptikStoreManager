using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Database;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<RetailChain> RetailChains { get; set; }
    public DbSet<RetailStore> RetailStores { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        builder.Entity<RetailChain>();

        builder.Entity<RetailStore>();

        builder.Entity<ApplicationUser>(b =>
        {
            //The users table is renamed to profil as part of the requirement.
            b.ToTable("Profil");
            b.Property(e => e.Email).HasColumnName("EMail"); //Renamed as part of the requirement
        });

        builder.Entity<ApplicationUserClaim>(b =>
        {
            b.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<Guid>>(b =>
        {
            b.ToTable("UserLogins");
        });

        builder.Entity<IdentityUserToken<Guid>>(b =>
        {
            b.ToTable("UserTokens");
        });

        builder.Entity<ApplicationRole>(b =>
        {
            b.ToTable("Roles");
        });

        builder.Entity<IdentityRoleClaim<Guid>>(b =>
        {
            b.ToTable("RoleClaims");
        });

        builder.Entity<ApplicationUserRole>(b =>
        {
            b.ToTable("UserRoles");
        });
    }
}
