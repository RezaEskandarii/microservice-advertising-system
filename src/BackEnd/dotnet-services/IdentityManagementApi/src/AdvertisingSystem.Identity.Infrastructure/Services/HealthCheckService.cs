using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using AdvertisingSystem.Identity.Shared.Interfaces;

namespace AdvertisingSystem.Identity.Infrastructure.Services;

public class HealthCheckService : IHealthCheckService
{
    private readonly ApplicationDbContext _context;

    public HealthCheckService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> CanConnectToDBAsync()
    {
        return _context.Database.CanConnectAsync();
    }
}