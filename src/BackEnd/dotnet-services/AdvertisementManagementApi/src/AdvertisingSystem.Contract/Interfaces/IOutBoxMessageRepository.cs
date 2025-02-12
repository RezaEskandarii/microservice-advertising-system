using AdvertisingSystem.Domain.Entities;

namespace AdvertisingSystem.Contract.Interfaces;

public interface IOutBoxMessageRepository
{
    public void AddOutboxMessage(string eventType, string eventData);
    Task<ICollection<OutBoxMessage>> GetUnProcessedMessagesAsync(string messageType);
    Task MarkAsProcessedAsync(OutBoxMessage message);
}