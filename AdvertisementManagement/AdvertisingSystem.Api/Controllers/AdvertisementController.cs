using System.Threading.Tasks;
using AdvertisingSystem.Application.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AdvertisementController : BaseController
{
    private readonly IMediator _mediator;

    public AdvertisementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync()
    {
        await _mediator.Send(new CreateAdvertisementCommand());
        return Ok();
    }
}