namespace AdvertisingSystem.Domain.DomainEvents;

public class AdvertisementRemovedEvent : IDomainEvent
{
    public AdvertisementRemovedEvent(long advertisementIdf)
    {
        AdvertisementIdf = advertisementIdf;
    }

    public object Data { get; set; }
    public long AdvertisementIdf { get; set; }
}