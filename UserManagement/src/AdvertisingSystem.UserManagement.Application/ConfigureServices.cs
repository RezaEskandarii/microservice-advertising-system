using System.Reflection;
using AdvertisingSystem.UserManagement.Application.Services;
using AdvertisingSystem.UserManagement.Contract.Interfaces;
using AdvertisingSystem.UserManagement.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.UserManagement.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddAutoMapper(Assembly.LoadFrom("AdvertisingSystem.UserManagement.Application.Profiles"));

        services.AddScoped<IUserAppService, UserAppService>();
    }
}