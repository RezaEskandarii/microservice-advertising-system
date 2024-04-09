using MediatR;

namespace AdvertisingSystem.Application.UseCases.Commands;

public record DeleteAdvertisementCommand(long Id,string? UserId) : IRequest;