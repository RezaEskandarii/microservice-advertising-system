namespace AdvertisingSystem.Contract.Interfaces;

public interface ISecretManager
{
    public Task<string> GetConnectionStringAsync();
}