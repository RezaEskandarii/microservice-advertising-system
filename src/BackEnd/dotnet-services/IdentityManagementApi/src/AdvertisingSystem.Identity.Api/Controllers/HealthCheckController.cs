using AdvertisingSystem.Identity.Shared.Interfaces;
using AdvertisingSystem.Identity.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly IDistributedTracer _tracer;

    public HealthCheckController(IHealthCheckService healthCheckService, IDistributedTracer tracer)
    {
        _healthCheckService = healthCheckService;
        _tracer = tracer;
    }


    [HttpGet("")]
    public async Task<ActionResult> CheckAsync()
    {
        _tracer.LogTraces(new TracingRequest()
        {
            Route = HttpContext.Request.PathBase,
            IpAddress = HttpContext.Request.ContentType,
            RequestID = "fgfsfsdfsdfsdfsdf"
        });
        return Ok(new
        {
            DBPing = await _healthCheckService.CanConnectToDBAsync()
        });
    }
}