using AdvertisingSystem.UserManagement.DbMigrator;
using AdvertisingSystem.UserManagement.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

        //setup our DI
        var serviceCollection = new ServiceCollection();

        ConfigureServices(serviceCollection, configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();


        using (var scope = serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();


            try
            {
                logger.LogInformation("Applying available migrations...");

                DbInitializer.ApplyMigrations(services).Wait();

                logger.LogInformation("Migrations done!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        Console.ReadKey();
    }


    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
    }
}