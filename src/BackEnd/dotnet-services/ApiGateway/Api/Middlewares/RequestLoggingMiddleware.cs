using System.Diagnostics;
using Serilog;

namespace Api.Middlewares;

public readonly record struct RequestLog
{
    public string TraceId { get; init; }
    public string Method { get; init; }
    public string Path { get; init; }
    public string QueryString { get; init; }
    public string UserAgent { get; init; }
    public string IpAddress { get; init; }
    public int? ResponseStatusCode { get; init; }
    public long ResponseTimeMs { get; init; }
    public string UserId { get; init; }
}

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

        Log.Information("Request Info: {@LogEntry}", logEntry);
    }
}