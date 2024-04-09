namespace AdvertisingSystem.Identity.Domain.DomainEvents;

public class UserCreatedEvent : IDomainEvent
{
    public object Data { get; }
    public string Title { get; }

    public UserCreatedEvent(object data, string title)
    {
        Data = data;
        Title = title;
    }
}