using System.Net;
using System.Text;
using System.Text.Json;
using AdvertisingSystem.UserManagement.Api.ViewModels;
using AdvertisingSystem.UserManagement.Shared.Exceptions;

namespace AdvertisingSystem.UserManagement.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private const char NextLine = '\n';

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        this._next = next;
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

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        const string defaultErrorMessage = "please try again";
        var errorMessages = new List<string>();

        switch (ex)
        {
            // check if the exception is of a specific type and update the status code and message accordingly
            case BusinessException exception:
            {
                statusCode = HttpStatusCode.BadRequest;
                if (exception.Errors.Any())
                {
                    errorMessages = exception.Errors.ToList();
                }
                else
                {
                    errorMessages.Add(exception.Message);
                }

                break;
            }
        }

        if (!errorMessages.Any())
        {
            errorMessages.Add(defaultErrorMessage);
        }

        LogExceptions(ex, context);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var respObj = JsonSerializer.Serialize(new ApiResponse(statusCode)
        {
            ErrorMessages = errorMessages,
        }, serializeOptions);

        return context.Response.WriteAsync(respObj);
    }

    private static void LogExceptions(Exception exception, HttpContext context)
    {
        var logStr = new StringBuilder();
        var requestIp = context.Connection.RemoteIpAddress;
        var currentDateTime = DateTime.Now;
        logStr.Append(NextLine);
        logStr.Append($"############## User: {GetCurrentUser(context)} # IP:{requestIp} ###########################");
        logStr.Append(NextLine);
        logStr.Append($"######### DateTime: {currentDateTime} ## LocalDateTim: {currentDateTime} ###");
        logStr.Append(NextLine);
        logStr.Append($"############################# Route: {context.Request.Path} #################################");
        logStr.Append(NextLine);
        logStr.Append($"############################# QueryParams: {GetQueryParams(context)} ########################");
        logStr.Append(NextLine);
        logStr.Append($"### ExceptionMessage: {exception.Message}###");
        logStr.Append(NextLine);
        logStr.Append(exception.StackTrace);
        logStr.Append(NextLine);
        logStr.Append("########################################## END ###############################################");
        logStr.Append(NextLine);
    }

    private static string? GetCurrentUser(HttpContext context)
    {
        var currentUser = " unauthenticated user ";
        if (context.User is { Identity: not null })
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