namespace AdvertisingSystem.Identity.Application.UseCases.Commands;

public class LoginResponse
{
    public string AuthToken { get; set; }
    public DateTime AuthTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}