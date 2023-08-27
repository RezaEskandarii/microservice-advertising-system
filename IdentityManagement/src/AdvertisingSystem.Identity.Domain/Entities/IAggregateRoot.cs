using AdvertisingSystem.Identity.Domain.DomainEvents;

namespace AdvertisingSystem.Identity.Domain.Entities;

public interface IAggregateRoot<TID> : IEntity<TID>
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void AddDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}