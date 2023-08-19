using System.Text;
using System.Text.Json;
using AdvertisingSystem.Identity.Domain.DomainEvents;
using AdvertisingSystem.Identity.Domain.Interfaces;
using AdvertisingSystem.Identity.Shared.Interfaces;
using RabbitMQ.Client;

namespace AdvertisingSystem.Identity.Infrastructure.Messaging.EventPublisher;

public class EventPublisher : IEventPublisher
{
    private readonly ISecretManager _secretManager;

    public EventPublisher(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }

    public async Task PublishAsync(IDomainEvent @event, string routingKey, string exchange, string queue)
    {
        try
        {
            var message = JsonSerializer.Serialize(@event.Data);
            // RabbitMQ connection string
            var connString = await _secretManager.ReadSecretAsync("RabbitMqSecret");

            // Create connection factory
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(connString)
            };

            // Create connection
            using var connection = factory.CreateConnection();
            // Create channel
            using var channel = connection.CreateModel();
            // Declare the exchange
            channel.ExchangeDeclare(
                exchange: exchange, // exchange name
                type: "topic", // exchange type
                durable: true // durable
            );

            // Message properties
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            // Convert the message to bytes
            var body = Encoding.UTF8.GetBytes(message);

            // Publish the message to the exchange
            channel.BasicPublish(exchange, routingKey, properties, body);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}