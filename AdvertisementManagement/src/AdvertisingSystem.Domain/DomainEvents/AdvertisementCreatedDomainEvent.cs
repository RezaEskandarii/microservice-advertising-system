namespace AdvertisingSystem.Domain.DomainEvents;

public class AdvertisementCreatedDomainEvent : IDomainEvent
{
    public object Data { get; set; }

    public AdvertisementCreatedDomainEvent()
    {
    }

    public AdvertisementCreatedDomainEvent(object data)
    {
        Data = data;
    }
}