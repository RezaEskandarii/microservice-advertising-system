using System.Text.Json;

namespace FrontEnd.Extensions;

public static class Http
{
    public static async Task<T?> DeserializeContentAsync<T>(this HttpResponseMessage response)
    {
        var contentStream = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(contentStream);
    }
}