using AdvertisingSystem.UserManagement.Contract.Dtos.User;
using AdvertisingSystem.UserManagement.Contract.Interfaces;
using AdvertisingSystem.UserManagement.Infrastructure;
using AdvertisingSystem.UserManagement.Infrastructure.Persistence.Context;
using AdvertisingSystem.UserManagement.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.UserManagement.DbMigrator;

public class DbInitializer
{
    public static async Task ApplyMigrations(IServiceProvider provider)
    {
        var context = provider.GetRequiredService<ApplicationDbContext>();
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }

    public static async Task SeedAsync(IServiceProvider provider)
    {
        var userService = provider.GetRequiredService<IUserAppService>();
        const string email = "admin@admin.com";
        var appUser = await userService.FindByUserNameAsync(email);
        if (appUser == null)
        {
            var adminUser = new CreateUserDto()
            {
                Email = email,
                Password = "Reza@@@@1111",
                ConfirmPassword = "Reza@@@@1111",
                FirstName = "super admin",
                LastName = "admin",
                PhoneNumber = "00",
                CellNumber = "44",
                Address = "Canada",
                Role = UserRoles.SuperAdmin
            };
            await userService.CreateAsync(adminUser);
        }
    }
}