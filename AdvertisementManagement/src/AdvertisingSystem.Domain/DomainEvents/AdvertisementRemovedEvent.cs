namespace AdvertisingSystem.Domain.DomainEvents;

public class AdvertisementRemovedEvent : IDomainEvent
{
    public AdvertisementRemovedEvent(long advertisementId)
    {
        AdvertisementId = advertisementId;
        Id = Guid.NewGuid();
    }

    public object Data { get; set; }
    public Guid Id { get; set; }
    public long AdvertisementId { get; set; }
}