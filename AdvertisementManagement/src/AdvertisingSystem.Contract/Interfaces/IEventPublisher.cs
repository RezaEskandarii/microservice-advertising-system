using AdvertisingSystem.Domain.DomainEvents;

namespace AdvertisingSystem.Contract.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent @event);
}