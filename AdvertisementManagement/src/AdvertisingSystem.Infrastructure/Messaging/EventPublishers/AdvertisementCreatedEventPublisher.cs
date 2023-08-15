
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.DomainEvents;

namespace AdvertisingSystem.Infrastructure.Messaging.EventPublishers;

public class AdvertisementCreatedEventPublisher : IEventPublisher
{
    public Task PublishAsync(IDomainEvent @event)
    {
        throw new NotImplementedException();
    }
}