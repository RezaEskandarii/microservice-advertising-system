namespace Api.Tests.IntegrationTests.Models;

public class SignInResp
{
    public string AuthToken { get; set; }
    public DateTime AuthTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}