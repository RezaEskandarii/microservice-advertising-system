namespace AdvertisingSystem.Domain.DomainEvents;

public class AdvertisementCreatedDomainEvent : IDomainEvent
{
    public object Data { get; }
    public Guid Id { get; }

    public AdvertisementCreatedDomainEvent(Guid id, object data)
    {
        Id = id;
        Data = data;
    }
}