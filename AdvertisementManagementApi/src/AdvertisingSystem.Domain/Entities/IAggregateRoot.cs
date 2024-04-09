using AdvertisingSystem.Domain.DomainEvents;

namespace AdvertisingSystem.Domain.Entities;

public interface IAggregateRoot<T> : IEntity<T>
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void AddDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}