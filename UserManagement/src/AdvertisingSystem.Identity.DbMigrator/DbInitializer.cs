using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisingSystem.Identity.DbMigrator;

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
        var context = provider.GetRequiredService<ApplicationDbContext>();
        // const string email = "admin@admin.com";
        // var appUser = await context.Users.FirstOrDefaultAsync(x => x.Email.Value == email);
        // if (appUser == null)
        // {
        //     var adminUser = new AppUser()
        //     {
        //         Email = new Email(email),
        //         Password = "Reza@@@@1111",
        //         ConfirmPassword = "Reza@@@@1111",
        //         FirstName = "super admin",
        //         LastName = "admin",
        //         PhoneNumber = "00",
        //         CellNumber = "44",
        //         Address = "Canada",
        //         Role = UserRoles.SuperAdmin
        //     };
        //     await userService.CreateAsync(adminUser);
    }
}