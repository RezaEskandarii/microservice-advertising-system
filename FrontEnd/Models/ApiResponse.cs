namespace FrontEnd.Models;

public class ApiResponse<T>
{
    public T responseObject { get; set; }
    public List<string>? errorMessages { get; set; } = new List<string>();
    public object message { get; set; }
    public int statusCode { get; set; }
    public DateTime dateTime { get; set; }
}

public class SignInResponse
{
    public string authToken { get; set; }
    public DateTime authTokenExpiresAt { get; set; }
    public string refreshToken { get; set; }
    public DateTime refreshTokenExpiresAt { get; set; }
}