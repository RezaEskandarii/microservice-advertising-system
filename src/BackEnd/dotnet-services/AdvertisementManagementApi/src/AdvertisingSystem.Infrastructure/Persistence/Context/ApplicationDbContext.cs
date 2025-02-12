using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingSystem.Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new AdvertisementConfiguration());
        base.OnModelCreating(builder);
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var modifiedEntities = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .OfType<Advertisement>(); // Filter for Advertisement entities

        foreach (var entity in modifiedEntities)
        {
            // Set isSynced to false for Advertisement entities being updated
            entity.UpdateIsSyncedInReadDb(false);
        }

        return base.SaveChangesAsync(cancellationToken);
    }


    public DbSet<Advertisement> Advertisements { get; set; }
    public DbSet<OutBoxMessage> OutBoxMessages { get; set; }
}