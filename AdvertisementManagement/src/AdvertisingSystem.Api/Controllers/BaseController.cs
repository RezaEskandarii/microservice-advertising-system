using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace AdvertisingSystem.Api.Controllers;

public class BaseController : ControllerBase
{
    private readonly ILogger<BaseController> _logger;

    public BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;
    }

    protected string ExtractUserIdFromJwt()
    {
        var requestHeader = StringValues.Empty;

        try
        {
            if (Request.Headers.TryGetValue("Authorization", out requestHeader))
            {
                var token = requestHeader.FirstOrDefault()
                    ?.Replace("Bearer", "")
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

            throw new AggregateException($"could process JWT token: {requestHeader.FirstOrDefault()}");
        }

        throw new ArgumentException("user id is null in jwt claims");
    }
}