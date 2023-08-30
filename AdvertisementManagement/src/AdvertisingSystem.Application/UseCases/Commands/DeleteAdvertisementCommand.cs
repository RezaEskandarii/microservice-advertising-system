using MediatR;

namespace AdvertisingSystem.Application.UseCases.Commands;

public class DeleteAdvertisementCommand : IRequest
{
    public long Id { get; set; }
    public string? UserId { get; set; }
}