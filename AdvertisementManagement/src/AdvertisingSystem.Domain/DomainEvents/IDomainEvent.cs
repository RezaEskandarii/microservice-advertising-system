namespace AdvertisingSystem.Domain.DomainEvents;

public interface IDomainEvent
{
    public object Data { get; set; }
    public Guid Id { get; set; }
}