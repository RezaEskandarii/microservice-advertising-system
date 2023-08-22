using System.Text.Json;
using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.DomainEvents;
using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Domain.ValueObjects;
using File;
using Google.Protobuf;
using Grpc.Net.Client;
using MediatR;

namespace AdvertisingSystem.Application.Handlers.CommandHandlers;

public class CreateAdvertisementCommandHandler : IRequestHandler<CreateAdvertisementCommand>
{
    private readonly IAdvertisementRepository _advertisementRepository;
    private readonly IEventPublisher _eventPublisher;

    public CreateAdvertisementCommandHandler(IAdvertisementRepository advertisementRepository,
        IEventPublisher eventPublisher)
    {
        _advertisementRepository = advertisementRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(CreateAdvertisementCommand command, CancellationToken cancellationToken)
    {
        var advertisement = Advertisement.CreateNew(command.Title, command.UserId, command.Description, command.Price,
            new CreateDate(DateTime.Now), new UpdateDate(DateTime.Now), new ExpiryDate(command.ExpiresAt),
            command.Address, command.CategoryId);
        
        await UploadImagesAsync(command);
        var result = await _advertisementRepository.AddAsync(advertisement);

        var obj = new { Title = result.Title, UserEmail = "" };
        var @event = new AdvertisementCreatedDomainEvent(JsonSerializer.Serialize(obj));
        await _eventPublisher.PublishAsync(@event, "ad_events.OnAdvertisementAdded", "ad_events", "email_queue");
    }

    private async Task UploadImagesAsync(CreateAdvertisementCommand command)
    {
        // Create a gRPC channel and client
        // TODO: read address from service discovery
        var channel = GrpcChannel.ForAddress("http://localhost:5002");
        var client = new FileService.FileServiceClient(channel);

        // Read the image file into a byte array

        if (command.Thumbnails != null)
            foreach (var fileBytes in command.Thumbnails)
            {
                // Create the request message
                var request = new FileRequest
                {
                    FileContent = ByteString.CopyFrom(fileBytes),
                    FileName = "image.jpg",
                    AdvertisementId = 123
                };

                // Call the gRPC method
                var response = await client.UploadFileAsync(request);
            }
    }
}