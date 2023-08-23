using System.Reflection;
using AdvertisingSystem.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddInfraStructureServices();
        
        return services;
    }
}