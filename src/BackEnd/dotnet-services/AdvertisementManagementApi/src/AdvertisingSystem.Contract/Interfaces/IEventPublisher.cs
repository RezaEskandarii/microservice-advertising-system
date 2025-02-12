using AdvertisingSystem.Domain.DomainEvents;

namespace AdvertisingSystem.Contract.Interfaces;

public interface IEventPublisher
{
    /// <summary>
    /// publish message to message broker
    /// </summary>
    /// <param name="event"></param>
    /// <param name="routingKey"></param>
    /// <param name="exchange"></param>
    /// <param name="queue"></param>
    /// <returns></returns>
    Task PublishAsync(IDomainEvent @event, string routingKey, string exchange, string queue);
}