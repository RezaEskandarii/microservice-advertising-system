namespace AdvertisingSystem.Identity.Domain.DomainEvents;

public interface IDomainEvent
{
    public object Data { get; }
}