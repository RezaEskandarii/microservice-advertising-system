using AdvertisingSystem.Identity.Application.UseCases.Commands;
using AdvertisingSystem.Identity.Application.UseCases.Queries;
using AdvertisingSystem.Identity.Domain.DomainEvents;
using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Domain.Interfaces;
using AdvertisingSystem.Identity.Shared.Enums;
using MediatR;

namespace AdvertisingSystem.Identity.Application.Handlers.CommandHandlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, GetUser>
{
    private readonly IUserRepository _repository;

    public CreateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetUser> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var appUser = AppUser.CreateNew(new(command.FirstName), new(command.LastName),
            new(command.PhoneNumber), new(command.Email), command.Address, UserStatuses.Enable);

        var result = await _repository.CreateAsync(appUser);
        appUser.AddDomainEvent(new UserCreatedEvent(appUser));
        return new GetUser();
    }
}