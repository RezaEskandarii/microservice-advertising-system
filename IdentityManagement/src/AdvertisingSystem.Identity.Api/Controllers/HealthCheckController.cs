using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Text;

namespace AdvertisingSystem.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly ApplicationDbContext _appContext;

    void x_dot_()
    {
        _appContext.Database.CanConnect();
    }
}