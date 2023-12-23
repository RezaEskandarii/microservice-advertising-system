using AdvertisingSystem.Application.UseCases.Queries;
using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain;
using AutoMapper;
using MediatR;

namespace AdvertisingSystem.Application.Handlers.QueryHandlers;

public class
    SearchAdvertisementQueryHandler : IRequestHandler<SearchAdvertisementQuery, PaginatedList<GetAdvertisementViewModel>>
{
    private readonly IAdvertisementRepository _advertisementRepository;
    private readonly IMapper _mapper;

    public SearchAdvertisementQueryHandler(IMapper mapper, IAdvertisementRepository advertisementRepository)
    {
        _mapper = mapper;
        _advertisementRepository = advertisementRepository;
    }

    public async Task<PaginatedList<GetAdvertisementViewModel>> Handle(SearchAdvertisementQuery request,
        CancellationToken cancellationToken)
    {
        var advertisements = await _advertisementRepository.SearchAsync(request.PageNumber,request.PageSize,request.Filter);
        
        return _mapper.Map<PaginatedList<GetAdvertisementViewModel>>(advertisements);
    }
}