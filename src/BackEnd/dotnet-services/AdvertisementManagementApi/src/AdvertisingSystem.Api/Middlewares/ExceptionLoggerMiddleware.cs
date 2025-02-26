using System.Net;
using System.Text.Json;
using AdvertisingSystem.Api.ViewModels;

namespace AdvertisingSystem.Api.Middlewares;

public class ExceptionLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionLoggerMiddleware> _logger;

    public ExceptionLoggerMiddleware(RequestDelegate next, ILogger<ExceptionLoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        _logger.LogError(exception, "Exception occurred - TraceId: {TraceId}, Path: {Path}, Method: {Method}, IP: {Ip}", 
            traceId, requestPath, requestMethod, ipAddress);

        var errorResponse = new ApiResponse<string>(
            HttpStatusCode.InternalServerError,
            "An unexpected error occurred.",
            null,
            null,
            new ApiError
            {
                Type = exception.GetType().Name,
                Title = "Unhandled Exception",
                Status = HttpStatusCode.InternalServerError,
                Detail = exception.Message,
                Instance = context.Request.Path
            }
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        await context.Response.WriteAsync(jsonResponse);
    }
}
