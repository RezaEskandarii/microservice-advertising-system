using AdvertisingSystem.Contract.Interfaces;

namespace AdvertisingSystem.Infrastructure.ExternalServices;

public class SecretManager : ISecretManager
{
    public async Task<string> GetConnectionStringAsync()
    {
        return await Task.FromResult(
            "Server=127.0.0.1;Port=5432;Database=AdvertisingSystemDB;User Id=postgres;Password=boofhichkas");
    }
}