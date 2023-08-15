using System.Text;
using System.Text.Json;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.DomainEvents;
using RabbitMQ.Client;

namespace AdvertisingSystem.Infrastructure.Messaging.EventPublishers;

public class AdvertisementCreatedEventPublisher : IEventPublisher
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly ISecretManager _secretManager;

    public AdvertisementCreatedEventPublisher(ISecretManager secretManager)
    {
        _secretManager = secretManager;
        // Connection settings
        //TODO: read configs from secret manager
        _connectionFactory = new ConnectionFactory
        {
            HostName = "localhost", // or provide the RabbitMQ server host name
            UserName = "guest", // RabbitMQ username
            Password = "guest" // RabbitMQ password
        };
    }


    public Task PublishAsync(IDomainEvent @event, string routingKey, string exchange, string queue)
    {
        // Create a connection to RabbitMQ
        using var connection = _connectionFactory.CreateConnection();
        // Create a channel
        using var channel = connection.CreateModel();
        // Declare a queue
        channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        var message = JsonSerializer.Serialize(@event.Data);

        // Convert the message to bytes
        var body = Encoding.UTF8.GetBytes(message);

        // Publish the message to the queue
        channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: null, body: body);

        return Task.CompletedTask;
    }
}