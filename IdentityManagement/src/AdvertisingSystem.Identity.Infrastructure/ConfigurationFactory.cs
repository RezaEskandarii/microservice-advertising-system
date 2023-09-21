using Microsoft.Extensions.Configuration;

namespace AdvertisingSystem.Identity.Infrastructure;

public static class ConfigurationFactory
{
    public static IConfigurationRoot BuildNewConfigurationRoot()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();
    }
}