using System.Text;
using System.Text.Json;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.DomainEvents;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace AdvertisingSystem.Infrastructure.Messaging.EventPublishers;

public class AdvertisementCreatedEventPublisher : IEventPublisher
{
    private readonly IConfiguration _configuration;

    public AdvertisementCreatedEventPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishAsync(IDomainEvent @event, string routingKey, string exchange, string queue)
    {
        try
        {
            var message = JsonSerializer.Serialize(@event.Data);

            // Create connection factory
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration["ConnectionStrings:RabbitMq"])
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
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}