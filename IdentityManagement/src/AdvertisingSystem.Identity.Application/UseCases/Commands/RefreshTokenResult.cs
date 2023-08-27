namespace AdvertisingSystem.Identity.Application.UseCases.Commands;

public class RefreshTokenResult
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public RefreshTokenResult(){}
    public RefreshTokenResult(string token, DateTime expiresAt)
    {
        Token = token;
        ExpiresAt = expiresAt;
    }
}