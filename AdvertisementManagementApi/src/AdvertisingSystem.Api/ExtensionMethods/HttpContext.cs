using System.IdentityModel.Tokens.Jwt;
using AdvertisingSystem.Application.ViewModels;
using Microsoft.Extensions.Primitives;

namespace AdvertisingSystem.Api.ExtensionMethods;

public static class HttpContextMethods
{
    /// <summary>
    /// read user id from claims
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="AggregateException"></exception>
    public static string GetUserId(this HttpRequest request)
    {
        var requestHeader = StringValues.Empty;

        try
        {
            if (request.Headers.TryGetValue("Authorization", out requestHeader))
            {
                var token = requestHeader.FirstOrDefault()
                    ?.Replace("Bearer", "")
                    .Replace(" ", "");

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Retrieve the userId claim from the token's claims
                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "id");

                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    return userId;
                }
            }
            else
            {
                throw new ArgumentException("HttpRequest header does not contains Bearer token");
            }
        }
        catch (Exception e)
        {
            throw new AggregateException($"could process JWT token: {requestHeader.FirstOrDefault()}");
        }

        return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<ICollection<ThumbnailFileViewModel>?> GetThumbnailsAsync(this HttpRequest request)
    {
        var thumbnails = request.Form.Files.Where(x => x.Name == "Thumbnails").ToList();

        if (!thumbnails.Any())
            return null;

        var result = new List<ThumbnailFileViewModel>();
        foreach (var thumbnail in thumbnails)
        {
            using var memoryStream = new MemoryStream();
            await thumbnail.CopyToAsync(memoryStream);

            result.Add(new ThumbnailFileViewModel()
            {
                Bytes = memoryStream.ToArray(),
                FileName = thumbnail.FileName
            });
            memoryStream.Close();
        }

        return result;
    }
}