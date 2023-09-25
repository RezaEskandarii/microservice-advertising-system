using System.Net;
using AdvertisingSystem.Api.ViewModels;
using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Application.UseCases.Queries;
using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
        command.UserId = GetUserIdFromToken();
        command.Thumbnails = await GetThumbnailsFromRequestAsync();
      
        var result = await _mediator.Send(command);
        return Ok(new ApiResponse(HttpStatusCode.OK, result));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult> FindByIdAsync(long id)
    {
        var result = await _mediator.Send(new GetAdvertisementQuery() { Id = id });
        return Ok(new ApiResponse(HttpStatusCode.OK, result));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        await _mediator.Send(new DeleteAdvertisementCommand() { Id = id, UserId = GetUserIdFromToken() });
        return Ok(new ApiResponse(HttpStatusCode.OK));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] long id, [FromForm] UpdateAdvertisementCommand command)
    {
        command.AdvertsiementId = id;
        command.UserId = GetUserIdFromToken();
        command.Thumbnails = await GetThumbnailsFromRequestAsync();

        var result = await _mediator.Send(command);
        return Ok(new ApiResponse(HttpStatusCode.OK, result));
    }

    /// <summary>
    /// set thumbnails byte array
    /// </summary>
    /// <param name="command"></param>
    /// <param name="thumbnails"></param>
    private async Task<ICollection<ThumbnailFileViewModel>> GetThumbnailsFromRequestAsync()
    {
        var result = new List<ThumbnailFileViewModel>();
        var thumbnails = Request.Form.Files.Where(x => x.Name == "Thumbnails").ToList();

        if (!thumbnails.Any())
            return result;

        foreach (var thumbnail in thumbnails)
        {
            using var memoryStream = new MemoryStream();
            await thumbnail.CopyToAsync(memoryStream);

            result.Add(new ThumbnailFileViewModel()
            {
                Bytes = memoryStream.ToArray(),
                FileName = thumbnail.FileName
            });
            memoryStream.Close();
        }

        return result;
    }
}