using AdvertisingSystem.Contract.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.Infrastructure.Persistence.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddInfraStructureServices();

        var secretManager = serviceCollection.BuildServiceProvider().GetRequiredService<ISecretManager>();
        builder.UseNpgsql(secretManager.GetConnectionStringAsync().Result);

        return new ApplicationDbContext(builder.Options);
    }
}