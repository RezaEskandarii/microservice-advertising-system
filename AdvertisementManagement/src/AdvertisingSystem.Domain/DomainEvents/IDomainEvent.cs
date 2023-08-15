namespace AdvertisingSystem.Domain.DomainEvents;

public interface IDomainEvent
{
    public object Data { get; set; }
}