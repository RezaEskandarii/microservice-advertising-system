using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.Api.Controllers;

public class BaseController : ControllerBase
{
    private readonly ILogger<BaseController> _logger;

    public BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;
    }

    public string GetUserIdFromToken()
    {
        try
        {
            if (Request.Headers.TryGetValue("Authorization", out var headerValue))
            {
                var token = headerValue.FirstOrDefault()
                    .Replace("Bearer", "")
                    .Replace(" ", "");

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Retrieve the userId claim from the token's claims
                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "id");

                // Check if the userId claim exists
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    return userId;
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogDebug(e.Message);
            _logger.LogDebug(e.StackTrace);

            throw new Exception("could process JWT token");
        }

        // userId claim not found;
        throw new Exception("user id is null in jwt claims");
    }

    public string? GetRequestIDFromHeader()
    {
        // Read requestID from request header
        if (Request.Headers.TryGetValue("X-Request-ID", out var requestID))
        {
            return requestID;
        }

        return string.Empty;
    }
}