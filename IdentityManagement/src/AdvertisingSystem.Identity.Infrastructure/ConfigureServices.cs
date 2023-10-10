using System.Reflection;
using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using AdvertisingSystem.Identity.Infrastructure.Services;
using AdvertisingSystem.Identity.Shared.Interfaces;
using AdvertisingSystem.Identity.Shared.ViewModels;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Contrib.NetCore.Configuration;
using OpenTracing.Util;

namespace AdvertisingSystem.Identity.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddLogging(options => options.AddConsole())
            .AddSingleton<IConfiguration>(configuration);

        ConfigureJaegerTracer(services);

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
    private static void ConfigureJaegerTracer(IServiceCollection serviceCollection)
    {
        serviceCollection.AddOpenTracing();
        // Adds the Jaeger Tracer.
        serviceCollection.AddSingleton<ITracer>(sp =>
        {
            var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var reporter = new RemoteReporter.Builder().WithLoggerFactory(loggerFactory).WithSender(new UdpSender())
                .Build();
            var tracer = new Tracer.Builder(serviceName)
                // The constant sampler reports every span.
                .WithSampler(new ConstSampler(true))
                // LoggingReporter prints every reported span to the logging framework.
                .WithReporter(reporter)
                .Build();
            return tracer;
        });

        serviceCollection.Configure<HttpHandlerDiagnosticOptions>(options =>
            options.OperationNameResolver =
                request => $"{request.Method.Method}: {request?.RequestUri?.AbsoluteUri}");
    }

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