using AdvertisingSystem.Application.ViewModels;
using MediatR;

namespace AdvertisingSystem.Application.UseCases.Queries;

public class GetAdvertisementQuery : IRequest<GetAdvertisementViewModel>
{
    public long Id { get; set; }
}