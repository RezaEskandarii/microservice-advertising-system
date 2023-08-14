using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Infrastructure.Messaging.EventPublishers;
using AdvertisingSystem.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfraStructureServices(this IServiceCollection services)
    {
        services.AddScoped<IEventPublisher, AdvertisementCreatedEventPublisher>();
        services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
        return services;
    }
}