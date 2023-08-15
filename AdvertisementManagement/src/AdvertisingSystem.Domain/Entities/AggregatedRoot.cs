using AdvertisingSystem.Domain.DomainEvents;

namespace AdvertisingSystem.Domain.Entities;

public abstract class AggregateRoot : IEntity<long>, IAggregateRoot<long>
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

    public long Id { get; set; }
}