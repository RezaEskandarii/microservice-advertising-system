using AdvertisingSystem.Application.ViewModels;
using MediatR;

namespace AdvertisingSystem.Application.UseCases.Queries;

public class SearchAdvertisementQuery : SearchFilterBase, IRequest<ICollection<GetAdvertisementViewModel>>
{
    public long Id { get; set; }
}