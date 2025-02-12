namespace Api.Interfaces;

public interface ISecretManager
{
    Task<string> ReadAsync(string secretKey);
}