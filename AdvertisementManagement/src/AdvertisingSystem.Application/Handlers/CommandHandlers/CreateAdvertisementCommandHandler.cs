using System.Text.Json;
using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.DomainEvents;
using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Domain.ValueObjects;
using AutoMapper;
using File;
using Google.Protobuf;
using Grpc.Net.Client;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace AdvertisingSystem.Application.Handlers.CommandHandlers;

public class CreateAdvertisementCommandHandler : IRequestHandler<CreateAdvertisementCommand, GetAdvertisementViewModel>
{
    private readonly IAdvertisementRepository _advertisementRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public CreateAdvertisementCommandHandler(IAdvertisementRepository advertisementRepository,
        IEventPublisher eventPublisher, IMapper mapper, IConfiguration configuration)
    {
        _advertisementRepository = advertisementRepository;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<GetAdvertisementViewModel> Handle(CreateAdvertisementCommand command,
        CancellationToken cancellationToken)
    {
        var advertisement = Advertisement.CreateNew(command.Title, command.UserId, command.Description,
            new Price(command.Price, "USD"),
            new CreateDate(DateTime.Now), new UpdateDate(DateTime.Now), new ExpiryDate(command.ExpiresAt),
            command.Address, command.CategoryId, command.Tags, command.Properties, command.LocationId);

        var result = await _advertisementRepository.AddAsync(advertisement);
        UploadImagesAsync(command, result.Id);

        var obj = new { Title = result.Title, UserEmail = "" };
        var @event = new AdvertisementCreatedDomainEvent(JsonSerializer.Serialize(obj), Guid.NewGuid());

        _eventPublisher.PublishAsync(@event, "ad_events.OnAdvertisementAdded", "ad_events", "email_queue");

        return _mapper.Map<GetAdvertisementViewModel>(result);
    }

    private async Task UploadImagesAsync(CreateAdvertisementCommand command, long advertisementId)
    {
        if (!command.Thumbnails.Any())
            return;

        var thumbnailServiceAddr = _configuration["thumbnail-service"];

        // Create a gRPC channel and client
        var channel = GrpcChannel.ForAddress(thumbnailServiceAddr);
        var client = new FileService.FileServiceClient(channel);

        // Read the image file into a byte array
        foreach (var thumbnail in command.Thumbnails)
        {
            // Create the request message
            var request = new FileRequest
            {
                FileContent = ByteString.CopyFrom(thumbnail.Bytes),
                FileName = thumbnail.FileName,
                AdvertisementId = advertisementId
            };

            // Call the gRPC method
            var response = await client.UploadFileAsync(request);
            await _advertisementRepository.AddThumbnailAsync(advertisementId, response.FileName);
        }
    }
}