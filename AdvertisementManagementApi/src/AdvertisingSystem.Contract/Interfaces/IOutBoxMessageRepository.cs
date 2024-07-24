namespace AdvertisingSystem.Contract.Interfaces;

public interface IOutBoxMessageRepository
{
    public void AddOutboxMessage(string eventType, string eventData);
}