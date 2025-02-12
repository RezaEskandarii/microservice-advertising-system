namespace Api.Middlewares;

public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Generate a unique request ID
        var requestId = Guid.NewGuid().ToString();

        // Add the request ID to the request header
        context.Request.Headers.Add("X-Request-ID", requestId);

        // Pass the request to the next middleware
        await _next(context);
    }
}