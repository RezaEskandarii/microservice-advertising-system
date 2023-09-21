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

    public async Task<GetAdvertisementViewModel> Handle(UpdateAdvertisementCommand command,
        CancellationToken cancellationToken)
    {
        var advertisement = await _repository.GetByIdAsync(command.AdvertsiementId, command.UserId);

        advertisement.UpdateTitle(command.Title);
        advertisement.UpdateDescription(command.Description);
        advertisement.UpdatePrice(new Price(command.Price, "USDT"));
        advertisement.UpdateExpiresAt(new ExpiryDate(command.ExpiresAt));
        advertisement.UpdateAddress(command.Address);
        advertisement.UpdateCategoryId(command.CategoryId);

        if (command.Thumbnails.Any())
        {
            advertisement.UpdateThumbnails(command.Thumbnails.Select(t => t.FileName).ToArray());
        }

        advertisement.UpdateTags(command.Tags);

        var result = await _repository.UpdateAsync(advertisement);
        return _mapper.Map<GetAdvertisementViewModel>(result);
    }
}