using System.Diagnostics;
using Serilog;

namespace AdvertisingSystem.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
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

        Log.Information("Request Info: {@LogEntry}", logEntry);
    }
}