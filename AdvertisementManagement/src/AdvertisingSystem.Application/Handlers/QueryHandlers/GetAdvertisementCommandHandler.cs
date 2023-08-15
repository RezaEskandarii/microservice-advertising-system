using AdvertisingSystem.Application.UseCases.Queries;
using MediatR;

namespace AdvertisingSystem.Application.Handlers;

public class GetAdvertisementCommandHandler : IRequestHandler<GetAdvertisementQuery>
{
    public Task Handle(GetAdvertisementQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}