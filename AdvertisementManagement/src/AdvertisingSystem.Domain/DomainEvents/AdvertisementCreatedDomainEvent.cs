namespace AdvertisingSystem.Domain.DomainEvents;

public class AdvertisementCreatedDomainEvent : IDomainEvent
{
    public object Data { get; set; }
    public Guid Id { get; set; }

    public AdvertisementCreatedDomainEvent()
    {
    }

    public AdvertisementCreatedDomainEvent(object data,Guid id)
    {
        Data = data;
        Id = id;
    }
}