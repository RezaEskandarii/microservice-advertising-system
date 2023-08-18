using AdvertisingSystem.Identity.Shared.Interfaces;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace AdvertisingSystem.Identity.Infrastructure.Services;

public class SecretManager : ISecretManager
{
    private readonly IVaultClient _vaultClient;
    private readonly IConfiguration _configuration;

    public SecretManager(IConfiguration configuration)
    {
        _configuration = configuration;
        var authMethod = new TokenAuthMethodInfo(_configuration["Vault:Token"]);
        var vaultClientSettings = new VaultClientSettings(_configuration["Vault:Address"], authMethod);
        _vaultClient = new VaultClient(vaultClientSettings);
    }

    public async Task<string?> ReadSecretAsync(string secretKey)
    {
        try
        {
            Secret<SecretData> secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("");

            if (secret != null && secret.Data != null && secret.Data.Data.TryGetValue(secretKey, out var value))
            {
                if (value != null) return value.ToString();
            }

            return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<string?> GetConnectionStringAsync()
    {
        return await ReadSecretAsync("DBConnectionString");
    }
}