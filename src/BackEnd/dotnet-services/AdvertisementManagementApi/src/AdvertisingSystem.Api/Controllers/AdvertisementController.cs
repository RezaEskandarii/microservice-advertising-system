using System.Net;
using AdvertisingSystem.Api.ExtensionMethods;
using AdvertisingSystem.Api.ViewModels;
using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Application.UseCases.Queries;
using AdvertisingSystem.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AdvertisementController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdvertisementController> _logger;

    public AdvertisementController(IMediator mediator, ILogger<AdvertisementController> logger) : base(logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromForm] CreateAdvertisementCommand command)
    {
        command.UserId = Request.GetUserId();
        command.Thumbnails = await Request.GetThumbnailsAsync();

        var result = await _mediator.Send(command);

        return Ok(new ApiResponse<GetAdvertisementViewModel>(result));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult> FindByIdAsync(long id)
    {
        var result = await _mediator.Send(new GetAdvertisementQuery(id));
        return Ok(new ApiResponse<GetAdvertisementViewModel>(result));
    }

    [HttpGet("")]
    public async Task<ActionResult> SearchAsync()
    {
        var result = await _mediator.Send(new GetAdvertisementsListQuery());
        var respObj = new ApiResponse<IReadOnlyCollection<GetAdvertisementViewModel>>()
        {
            Status = HttpStatusCode.OK,
            Data = result.Items,
            Pagination = new Pagination
            {
                Page = result.PageNumber,
                PerPage = result.PageSize,
                PagesTotal = result.TotalPages,
                TotalItems = result.TotalCount,
            }
        };
        return Ok(respObj);
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        await _mediator.Send(new DeleteAdvertisementCommand(id, Request.GetUserId()));
        return Ok(new ApiResponse<object>(HttpStatusCode.OK));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] long id, [FromForm] UpdateAdvertisementCommand command)
    {
        command.AdvertsiementId = id;
        command.UserId = Request.GetUserId();
        command.Thumbnails = await Request.GetThumbnailsAsync();

        var result = await _mediator.Send(command);
        return Ok(new ApiResponse<GetAdvertisementViewModel>(result));
    }
}