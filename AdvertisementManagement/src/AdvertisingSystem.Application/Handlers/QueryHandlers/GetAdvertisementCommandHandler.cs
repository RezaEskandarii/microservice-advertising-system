using AdvertisingSystem.Application.UseCases.Queries;
using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Contract.Interfaces;
using AutoMapper;
using MediatR;

namespace AdvertisingSystem.Application.Handlers.QueryHandlers;

public class GetAdvertisementCommandHandler : IRequestHandler<GetAdvertisementQuery, GetAdvertisementViewModel>
{
    private readonly IAdvertisementRepository _repository;
    private readonly IMapper _mapper;

    public GetAdvertisementCommandHandler(IAdvertisementRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<GetAdvertisementViewModel> Handle(GetAdvertisementQuery query, CancellationToken cancellationToken)
    {
        var advertisement = await _repository.GetByIdAsync(query.Id);
        return _mapper.Map<GetAdvertisementViewModel>(advertisement);
    }
}