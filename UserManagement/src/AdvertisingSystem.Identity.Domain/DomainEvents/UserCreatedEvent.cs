namespace AdvertisingSystem.Identity.Domain.DomainEvents;

public class UserCreatedEvent : IDomainEvent
{
    public object Data { get; }

    public UserCreatedEvent(object data)
    {
        Data = data;
    }
}