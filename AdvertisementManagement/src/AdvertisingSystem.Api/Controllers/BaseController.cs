using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.Api.Controllers;

public class BaseController : Controller
{
    public string GetUserIdFromToken()
    {

        if (Request.Headers.TryGetValue("Authorization", out var headerValue))
        {
            string token = headerValue.FirstOrDefault().Replace("Bearer ", "");

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

        // userId claim not found;
        throw new Exception("user id is null in jwt claims");
    }
}