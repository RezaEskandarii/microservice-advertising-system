using System.Text.Json;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Constants;
using AdvertisingSystem.Domain.DomainEvents;
using AdvertisingSystem.Domain.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AdvertisingSystem.Infrastructure.Jobs;

public class OutboxProcessorJob : IJob
{
    private readonly IOutBoxMessageRepository _outBoxMessageRepository;
    private IEventPublisher _eventPublisher;
    private readonly ILogger<OutboxProcessorJob> _logger;

    public OutboxProcessorJob(
        IOutBoxMessageRepository outBoxMessageRepository,
        IEventPublisher eventPublisher,
        ILogger<OutboxProcessorJob> logger
    )
    {
        _outBoxMessageRepository = outBoxMessageRepository;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        var outboxMessages = await _outBoxMessageRepository.GetUnProcessedMessagesAsync(EventTypes.AdvertisementCreated);

        foreach (var message in outboxMessages)
        {
            var messageEntity = JsonSerializer.Deserialize<Advertisement>(message.Data);
            await SendMessage(messageEntity);
            await _outBoxMessageRepository.MarkAsProcessedAsync(message);
        }
    }

    private async Task SendMessage(Advertisement ad)
    {
        try
        {
            var obj = new { Title = ad.Title, UserEmail = "" };
            var @event = new AdvertisementCreatedEvent(Guid.NewGuid(), JsonSerializer.Serialize(obj));

            await _eventPublisher.PublishAsync(@event, "ad_events.OnAdvertisementAdded", "ad_events", "email_queue");
        }
        catch (Exception e)
        {
            LogExceptions(e);
        }
    }

    private void LogExceptions(Exception exception)
    {
        _logger.LogDebug(exception.Message);
        _logger.LogDebug(exception.StackTrace);

        if (exception.InnerException is not null)
        {
            _logger.LogDebug(exception.InnerException.Message);
            _logger.LogDebug(exception.InnerException.StackTrace);
        }
    }
}