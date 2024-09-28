using AdvertisingSystem.Application.UseCases.Queries;
using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain;
using AutoMapper;
using MediatR;

namespace AdvertisingSystem.Application.Handlers.QueryHandlers;

public class GetAdvertisementsHandler : IRequestHandler<GetAdvertisementsListQuery, PaginatedList<GetAdvertisementViewModel>>
{
    private readonly IAdvertisementRepository _advertisementRepository;
    private readonly IMapper _mapper;

    public GetAdvertisementsHandler(IAdvertisementRepository advertisementRepository, IMapper mapper)
    {
        _advertisementRepository = advertisementRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GetAdvertisementViewModel>> Handle(GetAdvertisementsListQuery request, CancellationToken cancellationToken)
    {
        var advertisements = await _advertisementRepository.GetListAsync(request.PageNumber, request.PageSize);
        return _mapper.Map<PaginatedList<GetAdvertisementViewModel>>(advertisements);
    }
}