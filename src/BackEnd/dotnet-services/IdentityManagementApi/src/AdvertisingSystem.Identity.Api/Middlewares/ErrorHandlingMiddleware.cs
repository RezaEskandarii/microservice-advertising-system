using System.Net;
using System.Text.Json;
using AdvertisingSystem.Identity.Api.ViewModels;
using AdvertisingSystem.Identity.Shared.Exceptions;

namespace AdvertisingSystem.Identity.Api.Middlewares;

public record struct LogEntry
{
    public string User { get; set; }
    public string IpAddress { get; set; }
    public DateTime RequestTime { get; set; }
    public string Route { get; set; }
    public string QueryParams { get; set; }
    public string ExceptionMessage { get; set; }
    public string StackTrace { get; set; }
}

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;


    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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


    public async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, errorMessages) = GetErrorDetails(ex);

        LogExceptions(ex, context);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ApiResponse(statusCode)
        {
            ErrorMessages = errorMessages
        };

        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var responseBody = JsonSerializer.Serialize(response, serializeOptions);

        await context.Response.WriteAsync(responseBody);
    }

    private (HttpStatusCode, List<string>) GetErrorDetails(Exception ex)
    {
        const string defaultErrorMessage = "Please try again";
        var errorMessages = new List<string>();
        var statusCode = HttpStatusCode.InternalServerError;

        if (ex is BusinessException businessException)
        {
            statusCode = HttpStatusCode.BadRequest;
            errorMessages = businessException.Errors.Any()
                ? businessException.Errors.ToList()
                : new List<string> { businessException.Message };
        }
        else
        {
            errorMessages.Add(defaultErrorMessage);
        }

        return (statusCode, errorMessages);
    }


    private void LogExceptions(Exception exception, HttpContext context)
    {
        var logEntry = new LogEntry
        {
            User = GetCurrentUser(context),
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            RequestTime = DateTime.Now,
            Route = context.Request.Path,
            QueryParams = GetQueryParams(context),
            ExceptionMessage = exception.Message,
            StackTrace = exception.StackTrace
        };

        _logger.LogError("Exception occurred: {@LogEntry}", logEntry);
    }

    private static string? GetCurrentUser(HttpContext context)
    {
        var currentUser = " unauthenticated user ";
        if (context.User?.Identity != null)
        {
            currentUser = $" {context.User.Identity.Name} ";
        }

        return currentUser;
    }

    private static string? GetQueryParams(HttpContext context)
    {
        try
        {
            return JsonSerializer.Serialize(context.Request.Query);
        }
        catch
        {
            return "";
        }
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}