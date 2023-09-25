using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Infrastructure.ExternalServices;
using AdvertisingSystem.Infrastructure.Messaging.EventPublishers;
using AdvertisingSystem.Infrastructure.Persistence.Context;
using AdvertisingSystem.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfraStructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        services.AddScoped<ISecretManager, SecretManager>();
        services.AddScoped<IEventPublisher, AdvertisementCreatedEventPublisher>();
        services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
        services.AddScoped<IServiceDiscovery, ServiceDiscovery>();

        MigrateAsync(services).Wait();
        return services;
    }

    private static async Task MigrateAsync(IServiceCollection serviceCollection)
    {
        var context = serviceCollection.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();
        var migrations = await context.Database.GetPendingMigrationsAsync();
        if (migrations != null && migrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }
}