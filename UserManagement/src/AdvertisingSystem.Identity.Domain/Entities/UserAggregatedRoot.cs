using AdvertisingSystem.Identity.Domain.DomainEvents;
using AdvertisingSystem.Identity.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace AdvertisingSystem.Identity.Domain.Entities;

public abstract class UserAggregatedRoot : IdentityUser<string>,IAggregateRoot<string>
{
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public string Id { get; set; }
    public CreatedAt CreatedAt { get; set; }
    public UpdatedAt UpdatedAt { get; set; }
}