using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.DomainEvents;
using MediatR;

namespace AdvertisingSystem.Application.Handlers.CommandHandlers;

public class DeleteAdvertisementCommandHandler : IRequestHandler<DeleteAdvertisementCommand>
{
    private readonly IAdvertisementRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public DeleteAdvertisementCommandHandler(IAdvertisementRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(DeleteAdvertisementCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id, request.UserId);
        _eventPublisher.PublishAsync(new AdvertisementRemovedEvent(request.Id), "", "", "");
    }
}