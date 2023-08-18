namespace AdvertisingSystem.Identity.Shared.Interfaces;

public interface ISecretManager
{
    Task<string?> ReadSecretAsync(string secretKey);
    Task<string?> GetConnectionStringAsync();
}