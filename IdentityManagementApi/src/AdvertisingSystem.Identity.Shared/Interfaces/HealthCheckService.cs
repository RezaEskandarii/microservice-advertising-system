namespace AdvertisingSystem.Identity.Shared.Interfaces;

public interface IHealthCheckService
{
    Task<bool> CanConnectToDBAsync();
}