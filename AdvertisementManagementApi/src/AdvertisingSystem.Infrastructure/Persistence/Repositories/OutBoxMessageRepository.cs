using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Infrastructure.Persistence.Context;

namespace AdvertisingSystem.Infrastructure.Persistence.Repositories;

public class OutBoxMessageRepository : IOutBoxMessageRepository
{
    private readonly ApplicationDbContext _context;

    public OutBoxMessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddOutboxMessage(string eventType, string eventData)
    {
        var outboxMessage = new OutBoxMessage()
        {
            Id = Guid.NewGuid(),
            OccurredOn = DateTime.Now,
            Type = eventType,
            Data = eventData,
            Processed = false
        };

        _context.OutBoxMessages.Add(outboxMessage);
    }
}