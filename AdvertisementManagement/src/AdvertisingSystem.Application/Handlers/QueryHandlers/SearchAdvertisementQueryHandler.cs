using AdvertisingSystem.Application.UseCases.Queries;
using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AdvertisingSystem.Application.Handlers.QueryHandlers;

public class
    SearchAdvertisementQueryHandler : IRequestHandler<SearchAdvertisementQuery, ICollection<GetAdvertisementViewModel>>
{
    private readonly IElasticsearchRepository<Advertisement> _elasticsearchRepository;
    private readonly IMapper _mapper;

    public SearchAdvertisementQueryHandler(IElasticsearchRepository<Advertisement> elasticsearchRepository,
        IMapper mapper)
    {
        _elasticsearchRepository = elasticsearchRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<GetAdvertisementViewModel>> Handle(SearchAdvertisementQuery request,
        CancellationToken cancellationToken)
    {
        var advertisements =
            await _elasticsearchRepository.SearchAsync(request.Filter, request.PageNumber, request.PageSize);

        return _mapper.Map<ICollection<GetAdvertisementViewModel>>(advertisements);
    }
}