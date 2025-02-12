using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public ActionResult Check()
    {
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        var port = Request.HttpContext.Connection.LocalPort;
        return Ok(new
        {
            status = HttpStatusCode.OK.ToString(),
            address = $"{ipAddress}:{port}"
        });
    }
}