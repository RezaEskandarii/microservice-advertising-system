using System.Reflection;
using AdvertisingSystem.UserManagement.Application.Services;
using AdvertisingSystem.UserManagement.Contract.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.UserManagement.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.LoadFrom("AdvertisingSystem.UserManagement.Application.Profiles"));

        services.AddScoped<IUserAppService, UserAppService>();
    }
}