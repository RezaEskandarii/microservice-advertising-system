using System.Reflection;
using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using AdvertisingSystem.Identity.Infrastructure.Services;
using AdvertisingSystem.Identity.Shared.Interfaces;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTracing;
using OpenTracing.Util;

namespace AdvertisingSystem.Identity.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddLogging(options => options.AddConsole())
            .AddSingleton<IConfiguration>(configuration);

        services.AddJaeger(configuration);

        services.AddScoped<IHealthCheckService, HealthCheckService>();
        services.AddScoped<IDistributedTracer, DistributedTracer>();
        services.AddSingleton<ISecretManager, SecretManager>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        MigrateAsync(services).Wait();

        return services;
    }


    // Configure Jaeger Tracer
    private static void AddJaeger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITracer>(serviceProvider =>
        {
            string serviceName = Assembly.GetEntryAssembly().GetName().Name;
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            ISampler sampler = new ConstSampler(sample: true);

            ITracer tracer = new Tracer.Builder(serviceName)
                .WithLoggerFactory(loggerFactory)
                .WithSampler(sampler)
                .Build();
            
            return tracer;
        });

      
       
    }

    /// <summary>
    /// migrate pending migrations
    /// </summary>
    /// <param name="serviceCollection"></param>
    private static async Task MigrateAsync(IServiceCollection serviceCollection)
    {
        var provider = serviceCollection.BuildServiceProvider();
        var context = provider.GetRequiredService<ApplicationDbContext>();

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }
}

public class JaegerConfig
{
    public bool IsEnabled { get; set; }
    public double SamplingRate { get; set; }
    public int Port { get; set; }
    public string? Host { get; set; }
}