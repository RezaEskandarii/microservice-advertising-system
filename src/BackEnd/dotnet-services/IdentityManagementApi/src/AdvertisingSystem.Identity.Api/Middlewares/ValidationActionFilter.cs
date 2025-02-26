using System.Net;
using AdvertisingSystem.Identity.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdvertisingSystem.Identity.Api.Middlewares;

public class ValidationActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.Method.ToUpper() == "GET") return;
        if (context.ModelState.IsValid) return;

        var message = string.Join(", ", GetModelStateErrors(context));

        var responseObj = new ApiResponse<object>(
            HttpStatusCode.BadRequest,
            message,
            null,
            null,
            new ApiError()
            {
                Detail = message,
                Instance = context.HttpContext.Request.Path,
                Status = HttpStatusCode.BadRequest,
                
            }
        );

        context.Result = new JsonResult(responseObj)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    private ICollection<string> GetModelStateErrors(ActionExecutingContext context)
    {
        return (
                from t in context.ModelState
                where t.Value is { Errors: not null } && t.Value.Errors.Any()
                from item in t.Value.Errors
                select item.ErrorMessage)
            .ToList();
    }
}