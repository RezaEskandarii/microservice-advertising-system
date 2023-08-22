using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Infrastructure.ExternalServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.Infrastructure.Persistence.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddScoped<ISecretManager, SecretManager>();

        var secretManager = serviceCollection.BuildServiceProvider().GetRequiredService<ISecretManager>();
        builder.UseNpgsql(secretManager.GetConnectionStringAsync().Result);

        return new ApplicationDbContext(builder.Options);
    }
}