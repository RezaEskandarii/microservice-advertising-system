using MediatR;

namespace AdvertisingSystem.Application.UseCases.Queries;

public class GetAdvertisementQuery : IRequest<GetAdvertisement>
{
    public long Id { get; set; }
}