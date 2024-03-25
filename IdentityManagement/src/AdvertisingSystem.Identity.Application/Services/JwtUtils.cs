using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AdvertisingSystem.Identity.Application.Interfaces;
using AdvertisingSystem.Identity.Application.UseCases.Commands;
using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AdvertisingSystem.Identity.Application.Services;

public class JwtUtils : IJwtUtils
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public JwtUtils(UserManager<AppUser> userManager, ISecretManager secretManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<LoginResponse> GenerateJwtTokenAsync(AppUser user)
    {
        var tokenExpiresAt = DateTime.UtcNow.AddMinutes(15);
        var token = GenerateToken(tokenExpiresAt, user);

        var refreshToken = await GenerateRefreshTokenAsync(user.UserName, DateTime.Now.AddMonths(1));

        return new LoginResponse()
        {
            AuthToken = token,
            AuthTokenExpiresAt = tokenExpiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }


    #region Private

    private string GetUniqueToken()
    {
        var token = $"{Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))}{Guid.NewGuid()}";

        var tokenIsUnique = !_userManager.Users.Any(x => x.RefreshToken == token);

        if (!tokenIsUnique)
            return GetUniqueToken();

        return Regex.Replace(token, "[^a-zA-Z0-9]", string.Empty);
    }

    private string GenerateToken(DateTime tokenExpiresAt, AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKeyStr = _configuration["JWTSecretKey"];

        var key = Encoding.ASCII.GetBytes(secretKeyStr);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GetUserClaims(user),
            Expires = tokenExpiresAt,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private async Task<RefreshTokenResult> GenerateRefreshTokenAsync(string username, DateTime expiresAt)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (string.IsNullOrWhiteSpace(user.RefreshToken) || DateTime.UtcNow > user.RefreshTokenExpiresAt)
        {
            var refreshToken = GetUniqueToken();

            user.SetRefreshToken(refreshToken);
            user.SetRefreshTokenExpiresAt(expiresAt);

            await _userManager.UpdateAsync(user);
        }

        return new RefreshTokenResult(user.RefreshToken, expiresAt);
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