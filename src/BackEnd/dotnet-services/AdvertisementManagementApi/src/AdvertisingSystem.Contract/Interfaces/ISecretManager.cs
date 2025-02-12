namespace AdvertisingSystem.Contract.Interfaces;

public interface ISecretManager
{
    public Task<string> GetConnectionStringAsync();
    public Task<string> ReadSecretAsync(string key);
}