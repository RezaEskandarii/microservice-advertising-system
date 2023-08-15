using System.Text.Json;
using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.DomainEvents;
using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Domain.ValueObjects;
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
            new CreateDate(command.CreatedAt), new UpdateDate(command.UpdatedAt), new ExpiryDate(command.ExpiresAt),
            command.Address, command.CategoryId);
        var result = await _advertisementRepository.AddAsync(advertisement);

        var @event = new AdvertisementCreatedDomainEvent(JsonSerializer.Serialize(result));
        await _eventPublisher.PublishAsync(@event, "advertisements", "ad_events", "email_queue");
    }
}