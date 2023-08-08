using AdvertisingSystem.UserManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingSystem.UserManagement.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetCreateAndUpdateFields();
        return base.SaveChangesAsync(cancellationToken);
    }


    #region DbSets

    public DbSet<AppUser> Users { get; set; }

    #endregion


    #region Overrides

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    #endregion

    #region Privates

    private void SetCreateAndUpdateFields()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e is { Entity: EntityBase, State: EntityState.Added or EntityState.Modified });

        foreach (var entityEntry in entries)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    ((EntityBase)entityEntry.Entity).CreatedAt = DateTime.Now;
                    ((EntityBase)entityEntry.Entity).UpdatedAt = DateTime.Now;

                    break;
                case EntityState.Modified:
                    ((EntityBase)entityEntry.Entity).UpdatedAt = DateTime.Now;
                    break;
            }
        }
    }

    #endregion
}