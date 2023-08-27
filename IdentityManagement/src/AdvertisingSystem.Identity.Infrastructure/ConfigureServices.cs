using AdvertisingSystem.Identity.Domain.Interfaces;
using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using AdvertisingSystem.Identity.Infrastructure.Persistence.Repositories;
using AdvertisingSystem.Identity.Infrastructure.Services;
using AdvertisingSystem.Identity.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AdvertisingSystem.Identity.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddLogging(options => options.AddConsole())
            .AddSingleton<IConfiguration>(configuration);

        services.AddSingleton<ISecretManager, SecretManager>();

        // Build the service provider.
        IServiceProvider serviceProvider = services.BuildServiceProvider();

        // Get an instance of the interface from the service provider.
        var secretManager = serviceProvider.GetService<ISecretManager>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(secretManager.GetConnectionStringAsync().Result,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        return services;
    }
}