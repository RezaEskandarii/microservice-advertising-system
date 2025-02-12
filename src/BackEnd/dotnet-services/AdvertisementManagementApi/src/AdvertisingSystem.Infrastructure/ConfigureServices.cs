using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Infrastructure.ExternalServices;
using AdvertisingSystem.Infrastructure.Jobs;
using AdvertisingSystem.Infrastructure.Messaging.EventPublishers;
using AdvertisingSystem.Infrastructure.Persistence.Context;
using AdvertisingSystem.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace AdvertisingSystem.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfraStructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            );
        });

        var uriString = configuration["ElasticSearch:Url"] ?? throw new ArgumentException("elastic search connection string is null");
        var elasticClient = new ElasticClient(new Uri(uriString));

        services.AddSingleton<IElasticClient>(elasticClient);

        services.AddScoped<ISecretManager, SecretManager>();
        services.AddScoped<IEventPublisher, AdvertisementCreatedEventPublisher>();
        services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
        services.AddScoped<IServiceDiscovery, ServiceDiscovery>();
        services.AddScoped<IOutBoxMessageRepository, OutBoxMessageRepository>();
        services.AddScoped(typeof(IElasticsearchRepository<Advertisement>), typeof(AdvertisementElasticsearchRepository));

        MigrateAsync(services).Wait();

        services.RunSyncReadDatabaseJob();

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