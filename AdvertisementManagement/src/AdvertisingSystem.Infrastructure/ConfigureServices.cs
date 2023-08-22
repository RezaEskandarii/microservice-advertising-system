using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Infrastructure.ExternalServices;
using AdvertisingSystem.Infrastructure.Messaging.EventPublishers;
using AdvertisingSystem.Infrastructure.Persistence.Context;
using AdvertisingSystem.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfraStructureServices(this IServiceCollection services)
    {
        services.AddSingleton<ISecretManager, SecretManager>();
        var secretManager = services.BuildServiceProvider().GetRequiredService<ISecretManager>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(secretManager.GetConnectionStringAsync().Result,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });
        services.AddScoped<IEventPublisher, AdvertisementCreatedEventPublisher>();
        services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
        return services;
    }
}