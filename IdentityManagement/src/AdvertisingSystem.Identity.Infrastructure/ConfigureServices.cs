using System.Reflection;
using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using AdvertisingSystem.Identity.Infrastructure.Services;
using AdvertisingSystem.Identity.Shared.Interfaces;
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
using OpenTelemetry.Trace;

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
    public static void AddJaeger(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("JaegerConfig").Get<JaegerConfig>();

        if (!(config?.IsEnabled ?? false))
            return;

        if (string.IsNullOrEmpty(config?.Host))
            throw new Exception("invalid JaegerConfig");

        services.AddSingleton<ITracer>(serviceProvider =>
        {
            string serviceName = Assembly.GetEntryAssembly()?.GetName().Name;

            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            var sampler = new ProbabilisticSampler(config.SamplingRate);

            var reporter = new RemoteReporter.Builder()
                .WithLoggerFactory(loggerFactory)
                .WithSender(new UdpSender(config.Host, config.Port, 0))
                .WithFlushInterval(TimeSpan.FromSeconds(15))
                .WithMaxQueueSize(300)
                .Build();

            ITracer tracer = new Tracer.Builder(serviceName)
                .WithLoggerFactory(loggerFactory)
                .WithSampler(sampler)
                .WithReporter(reporter)
                .Build();

            GlobalTracer.Register(tracer);

            return tracer;
        });

        services.AddOpenTracing();
        
        services.opent()
            .WithTracing(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());
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

public class JaegerConfig
{
    public bool IsEnabled { get; set; }
    public double SamplingRate { get; set; }
    public int Port { get; set; }
    public string? Host { get; set; }
}