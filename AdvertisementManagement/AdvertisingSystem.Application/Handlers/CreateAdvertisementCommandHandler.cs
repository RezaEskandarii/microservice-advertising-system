using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Entities;
using MediatR;

namespace AdvertisingSystem.Application.Handlers;

public class CreateAdvertisementCommandHandler : IRequestHandler<CreateAdvertisementCommand>
{
    private readonly IAdvertisementRepository _advertisementRepository;
    private readonly IEventPublisher _eventPublisher;

    public CreateAdvertisementCommandHandler(IAdvertisementRepository advertisementRepository,
        IEventPublisher eventPublisher)
    {
        _advertisementRepository = advertisementRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(CreateAdvertisementCommand command, CancellationToken cancellationToken)
    {
        var advertisement = Advertisement.CreateNew(command.Title, command.UserId, command.Description, command.Price,
            command.CreatedAt, command.UpdatedAt, command.ExpiresAt, command.Address);
        await _advertisementRepository.AddAsync(advertisement);
        // await _eventPublisher.PublishAsync(new AdvertisementCreatedEvent(advertisement.Id, advertisement.Title));
    }
}