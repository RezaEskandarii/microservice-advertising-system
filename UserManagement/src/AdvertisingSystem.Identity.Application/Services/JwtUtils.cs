using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AdvertisingSystem.Identity.Application.Interfaces;
using AdvertisingSystem.Identity.Application.UseCases.Commands;
using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AdvertisingSystem.Identity.Application.Services;

public class JwtUtils : IJwtUtils
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ISecretManager _secretManager;

    public JwtUtils(UserManager<AppUser> userManager, ISecretManager secretManager)
    {
        _userManager = userManager;
        _secretManager = secretManager;
    }

    public async Task<LoginResponse> GenerateJwtToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKeyStr = await GetJwtSecretKey();
        var key = Encoding.ASCII.GetBytes(secretKeyStr);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GetUserClaims(user),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenStr = tokenHandler.WriteToken(token);

        var refreshToken = await GenerateRefreshToken(user.UserName, DateTime.Now.AddMonths(1));

        return new LoginResponse()
        {
            AuthToken = tokenStr,
            AuthTokenExpiresAt = tokenDescriptor.Expires.Value,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }


    #region Private

    private string GetUniqueToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenIsUnique = !_userManager.Users.Any(x => x.RefreshToken == token);

        if (!tokenIsUnique)
            return GetUniqueToken();

        return token;
    }

    private async Task<RefreshTokenResult> GenerateRefreshToken(string username, DateTime expiresAt)
    {
        var refreshToken = GetUniqueToken();
        var user = await _userManager.FindByNameAsync(username);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = expiresAt;

        await _userManager.UpdateAsync(user);

        return new RefreshTokenResult(refreshToken, expiresAt);
    }

    private async Task<string> GetJwtSecretKey()
    {
        return await _secretManager.GetJwtSecretKeyAsync();
    }

    private ClaimsIdentity GetUserClaims(AppUser user)
    {
        return new ClaimsIdentity(new[]
        {
            new Claim("id", user.Id.ToString()),
            new Claim("email", user.Email.Value),
            new Claim("firstName", user.FirstName.Value),
            new Claim("lastName", user.LastName.Value),
        });
    }

    #endregion
}