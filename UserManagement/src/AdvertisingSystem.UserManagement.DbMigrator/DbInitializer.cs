using AdvertisingSystem.UserManagement.Infrastructure;
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
}