using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.Api.Controllers;

public class BaseController : ControllerBase
{
    private readonly ILogger<BaseController> _logger;

    public BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;
    }
}