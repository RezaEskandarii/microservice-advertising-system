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
    public async Task<ActionResult> CreateAsync([FromForm] CreateAdvertisementCommand command,
        [FromForm] List<IFormFile> thumbnails)
    {
        await SetThumbnails(command, thumbnails);

        await _mediator.Send(command);
        return Ok();
    }

    /// <summary>
    /// set thumbnails byte array
    /// </summary>
    /// <param name="command"></param>
    /// <param name="thumbnails"></param>
    private static async Task SetThumbnails(CreateAdvertisementCommand command, List<IFormFile> thumbnails)
    {
        foreach (var thumbnail in thumbnails)
        {
            using var memoryStream = new MemoryStream();
            await thumbnail.CopyToAsync(memoryStream);
            command.Thumbnails?.Add(new ThumbnailFileModel()
            {
                Bytes = memoryStream.ToArray(),
                FileName = thumbnail.FileName
            });
            memoryStream.Close();
        }
    }
}