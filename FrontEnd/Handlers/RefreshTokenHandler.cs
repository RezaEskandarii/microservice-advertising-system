using System.Net;
using System.Net.Http.Headers;
using FrontEnd.Services;

namespace FrontEnd.Handlers;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;

    public RefreshTokenHandler(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {

        var accessToken = await _tokenService.GetAccessTokenAsync() ?? "";

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (!string.IsNullOrWhiteSpace(accessToken) && response.StatusCode == HttpStatusCode.Unauthorized)
        {
            accessToken = await _tokenService.GetAccessTokenAsync();

            // Set the new access token in the request header
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Retry the request with the new access token
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}