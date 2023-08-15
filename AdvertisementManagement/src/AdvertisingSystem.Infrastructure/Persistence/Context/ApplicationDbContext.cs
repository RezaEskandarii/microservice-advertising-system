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


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new AdvertisementConfiguration());
        base.OnModelCreating(builder);
    }

    public DbSet<Advertisement> Advertisements { get; set; }
}