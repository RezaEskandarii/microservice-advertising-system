using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Domain.ValueObjects;
using AdvertisingSystem.Identity.Infrastructure.EntityTypeConfigurations;
using AdvertisingSystem.Identity.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingSystem.Identity.Infrastructure.Persistence.Context;

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

    public override int SaveChanges()
    {
        SetCreateAndUpdateFields();
        return base.SaveChanges();
    }


    #region DbSets

    public DbSet<AppUser> Users { get; set; }

    #endregion


    #region Overrides

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    #endregion

    #region Privates

    private void SetCreateAndUpdateFields()
    {
        var entries = ChangeTracker
            .Entries<IAggregateRoot<string>>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            var entity = entityEntry.Entity;
            var now = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
                if (entity.DomainEvents.Any())
                {
                    foreach (var @event in entity.DomainEvents)
                    {
                        
                    }
                }
        
                entity.ClearDomainEvents();
            }

            entity.UpdatedAt = now;
        }
    }

    #endregion
}