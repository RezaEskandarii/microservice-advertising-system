using System.Reflection;
using System.Security.Claims;
using AdvertisingSystem.UserManagement.Domain.Entities;
using AdvertisingSystem.UserManagement.Infrastructure;
using AdvertisingSystem.UserManagement.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.UserManagement.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddAutoMapper(Assembly.Load("AdvertisingSystem.UserManagement.Application"));

        services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
                options.ClaimsIdentity.EmailClaimType = ClaimTypes.Email;
                options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
                
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}