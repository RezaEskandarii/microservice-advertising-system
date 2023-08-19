using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using AdvertisingSystem.Identity.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.Identity.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var serviceProvider = new ServiceCollection()
         
            .BuildServiceProvider();
        var secretManager = serviceProvider.GetRequiredService<ISecretManager>();

        builder.UseNpgsql(secretManager.GetConnectionStringAsync().Result);

        return new ApplicationDbContext(builder.Options);
    }
}