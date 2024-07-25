using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

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

    public async Task<ICollection<OutBoxMessage>> GetUnProcessedMessagesAsync(string messageType)
    {
        return await _context.OutBoxMessages
            .AsNoTracking()
            .Where(m => !m.Processed && m.Type == messageType)
            .ToListAsync();
    }

    public async Task MarkAsProcessedAsync(OutBoxMessage message)
    {
        var m = await _context.OutBoxMessages.FirstOrDefaultAsync(x => x.Id == message.Id);

        m.Processed = true;
        m.ProcessedOn = DateTime.Now;
        await _context.SaveChangesAsync();
    }
}