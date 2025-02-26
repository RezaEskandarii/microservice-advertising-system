using System.Diagnostics;

namespace AdvertisingSystem.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;
        var response = context.Response;

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        await _next(context);

        stopwatch.Stop();

        var logEntry = new RequestLog
        {
            TraceId = context.TraceIdentifier,
            Method = request.Method,
            Path = request.Path,
            QueryString = request.QueryString.ToString(),
            UserAgent = request.Headers["User-Agent"].ToString(),
            IpAddress = ipAddress,
            ResponseStatusCode = response.StatusCode,
            ResponseTimeMs = stopwatch.ElapsedMilliseconds
        };

        _logger.LogInformation("Request Info: {@LogEntry}", logEntry);
    }
}