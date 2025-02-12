using System.Text;
using System.Text.Json;

namespace Api.Tests.IntegrationTests;

public class TestClient : HttpClient
{
    private readonly string ApiAddress = "http://127.0.0.1:5009";
    private static string? JWTToken;

    public TestClient()
    {
        BaseAddress = new Uri(ApiAddress);
    }

    public new async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        AddJwtHeader(request);

        return await SendAsync(request);
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        AddJwtHeader(request);

        return await SendAsync(request);
    }

    public async Task<HttpResponseMessage> PutAsync(string requestUri, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        AddJwtHeader(request);

        return await SendAsync(request);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        AddJwtHeader(request);

        return await SendAsync(request);
    }


    public void SetJwtToken(string token)
    {
        JWTToken = token;
    }

    private void AddJwtHeader(HttpRequestMessage request)
    {
        if (!string.IsNullOrEmpty(JWTToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JWTToken);
        }
    }
}