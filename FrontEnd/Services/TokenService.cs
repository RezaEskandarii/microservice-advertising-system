using Blazored.LocalStorage;
using FrontEnd.Models;

namespace FrontEnd.Services;

public class TokenService
{
    private readonly ILocalStorageService _localStorageService;

    private const string AccessToken = "accessToken";
    private const string RefreshToken = "refreshToken";
    private const string AuthTokenExpiresAt = "authTokenExpiresAt";
    private const string RefreshTokenExpiresAt = "refreshTokenExpiresAt";

    public TokenService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }


    public async ValueTask<string?> GetAccessTokenAsync()
    {
        return await _localStorageService.GetItemAsync<string>(AccessToken);
    }

    public async Task SetAsync(SignInResponse resp)
    {
        await _localStorageService.SetItemAsStringAsync(AccessToken, resp.authToken);
        await _localStorageService.SetItemAsStringAsync(RefreshToken, resp.refreshToken);
        await _localStorageService.SetItemAsStringAsync(AuthTokenExpiresAt, resp.authTokenExpiresAt.ToString());
        await _localStorageService.SetItemAsStringAsync(RefreshTokenExpiresAt, resp.refreshTokenExpiresAt.ToString());
    }

    public async Task ClearAsync()
    {
        await _localStorageService.RemoveItemsAsync(new List<string>()
        {
            AccessToken,
            RefreshToken,
            AuthTokenExpiresAt,
            RefreshTokenExpiresAt
        });
    }

    public async ValueTask<bool> HasValidTokenAsync()
    {
        var expiresAt = await _localStorageService.GetItemAsStringAsync(RefreshTokenExpiresAt);
        return string.IsNullOrWhiteSpace(expiresAt) ? false : DateTime.Parse(expiresAt)  > DateTime.Now;
    }
}