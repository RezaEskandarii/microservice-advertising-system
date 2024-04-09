namespace AdvertisingSystem.Domain.DomainEvents;

public class AdvertisementCreatedEvent : IDomainEvent
{
    public object Data { get; }
    public Guid Id { get; }

    public AdvertisementCreatedEvent(Guid id, object data)
    {
        Id = id;
        Data = data;
    }
}