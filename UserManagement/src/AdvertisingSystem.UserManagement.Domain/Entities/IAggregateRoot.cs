using AdvertisingSystem.UserManagement.Domain.DomainEvents;

namespace AdvertisingSystem.UserManagement.Domain.Entities;

public interface IAggregateRoot<TID> : IEntity<TID>
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void AddDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}