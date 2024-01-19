using System.Reflection;
using System.Security.Claims;
using System.Text;
using AdvertisingSystem.Identity.Application.Behaviors;
using AdvertisingSystem.Identity.Application.Interfaces;
using AdvertisingSystem.Identity.Application.Services;
using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using AdvertisingSystem.Identity.Infrastructure;
using AdvertisingSystem.Identity.Shared.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AdvertisingSystem.Identity.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddScoped<IJwtUtils, JwtUtils>();
        services.AddScoped<IUserService, UserService>();
        services.AddInfrastructureServices(configuration);
        services.AddAutoMapper(Assembly.Load("AdvertisingSystem.Identity.Application"));

        var secretManager = services.BuildServiceProvider().GetRequiredService<ISecretManager>();

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

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = "http://127.0.0.1:5004",
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretManager.GetJwtSecretKeyAsync().Result))
                };
            });
    }
}