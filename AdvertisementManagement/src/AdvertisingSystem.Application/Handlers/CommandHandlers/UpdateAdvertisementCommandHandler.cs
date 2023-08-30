using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Application.UseCases.Queries;
using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.ValueObjects;
using AutoMapper;
using MediatR;

namespace AdvertisingSystem.Application.Handlers.CommandHandlers;

public class UpdateAdvertisementCommandHandler : IRequestHandler<UpdateAdvertisementCommand, GetAdvertisementViewModel>
{
    private readonly IAdvertisementRepository _repository;
    private readonly IMapper _mapper;

    public UpdateAdvertisementCommandHandler(IAdvertisementRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<GetAdvertisementViewModel> Handle(UpdateAdvertisementCommand command, CancellationToken cancellationToken)
    {
        var advertisement = await _repository.GetByIdAsync(command.AdvertsiementId, command.UserId);

        advertisement.Title = command.Title;
        advertisement.Description = command.Description;
        advertisement.Price = new Price(command.Price, "USDT");
        advertisement.ExpiresAt = new ExpiryDate(command.ExpiresAt);
        advertisement.Address = command.Address;
        advertisement.CategoryId = command.CategoryId;
        if (command.Thumbnails.Any())
        {
            advertisement.Thumbnails = command.Thumbnails.Select(t => t.FileName).ToArray();
        }
        advertisement.Tags = command.Tags;

        var result = await _repository.UpdateAsync(advertisement);
        return _mapper.Map<GetAdvertisementViewModel>(result);
    }
}