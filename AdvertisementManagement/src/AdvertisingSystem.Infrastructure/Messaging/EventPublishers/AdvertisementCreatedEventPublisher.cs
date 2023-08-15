using System.Text;
using System.Text.Json;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.DomainEvents;
using RabbitMQ.Client;

namespace AdvertisingSystem.Infrastructure.Messaging.EventPublishers;

public class AdvertisementCreatedEventPublisher : IEventPublisher
{
    private readonly ISecretManager _secretManager;

    public AdvertisementCreatedEventPublisher(ISecretManager secretManager)
    {
        _secretManager = secretManager;
    }


    public Task PublishAsync(IDomainEvent @event, string routingKey, string exchange, string queue)
    {
        try
        {
            var message = JsonSerializer.Serialize(@event.Data);
            // RabbitMQ connection string
            var connString = "amqp://guest:guest@localhost:5672/";

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
            channel.BasicPublish(
                exchange: exchange, // exchange name
                routingKey: routingKey, // routing key
                basicProperties: properties,
                body: body
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return Task.CompletedTask;
    }
}