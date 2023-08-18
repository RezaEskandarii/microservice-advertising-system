using AdvertisingSystem.UserManagement.Application.UseCases.Commands;
using AdvertisingSystem.UserManagement.Application.UseCases.Queries;
using AdvertisingSystem.UserManagement.Domain.DomainEvents;
using AdvertisingSystem.UserManagement.Domain.Entities;
using AdvertisingSystem.UserManagement.Domain.Interfaces;
using AdvertisingSystem.UserManagement.Shared.Enums;
using MediatR;

namespace AdvertisingSystem.UserManagement.Application.Handlers.CommandHandlers;

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
            new(command.PhoneNumber), command.Address, UserStatuses.Enable);

        var result = await _repository.CreateAsync(appUser);
        appUser.AddDomainEvent(new UserCreatedEvent(appUser));
        return new GetUser();
    }
}