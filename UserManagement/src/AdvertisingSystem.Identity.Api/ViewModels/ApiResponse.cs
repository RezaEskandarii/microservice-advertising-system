using System.Net;

namespace AdvertisingSystem.Identity.Api.ViewModels;

public class ApiResponse
{
    public ApiResponse(HttpStatusCode statusCode)
    {
        ErrorMessages = new List<string>();
        StatusCode = statusCode;
    }

    public ApiResponse()
    {
        ErrorMessages = new List<string>();
    }

    public object Item { get; set; }
    public ICollection<string> ErrorMessages { get; set; }
    public string Message { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public DateTime DateTime { get; set; } = DateTime.Now;
}