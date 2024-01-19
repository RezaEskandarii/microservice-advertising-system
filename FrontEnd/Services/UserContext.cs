using System.Text;
using System.Text.Json;
using FrontEnd.Models;

namespace FrontEnd.Services;

public class UserContext
{
    private readonly TokenService _tokenService;

    public UserContext(TokenService tokenService)
    {
        _tokenService = tokenService;
    }


    public async Task<User> CurrentUser()
    {
        var token = await _tokenService.GetAccessTokenAsync();
        var content = token.Split('.')[1];

        var jsonPayload = Encoding.UTF8.GetString(
            Convert.FromBase64String(content));
        
        return JsonSerializer.Deserialize<User>(jsonPayload);
    }
}