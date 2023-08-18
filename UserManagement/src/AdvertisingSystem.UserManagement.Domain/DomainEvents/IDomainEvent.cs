namespace AdvertisingSystem.UserManagement.Domain.DomainEvents;

public interface IDomainEvent
{
    public object Data { get; }
}