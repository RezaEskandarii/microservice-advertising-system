using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Domain;
using MediatR;

namespace AdvertisingSystem.Application.UseCases.Queries;

public class GetAdvertisementsListQuery : SearchFilterBase, IRequest<PaginatedList<GetAdvertisementViewModel>>
{
}

public class SearchAdvertisementQuery : SearchFilterBase, IRequest<PaginatedList<GetAdvertisementViewModel>>
{
    public long Id { get; set; }
    public Dictionary<string, string>? Properties { get; set; }
}