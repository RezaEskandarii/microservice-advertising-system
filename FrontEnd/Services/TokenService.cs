using Blazored.LocalStorage;

namespace FrontEnd.Services;

public class TokenService
{
    private readonly ILocalStorageService _localStorageService;

    public TokenService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await _localStorageService.GetItemAsync<string>("accessToken");
    }
}