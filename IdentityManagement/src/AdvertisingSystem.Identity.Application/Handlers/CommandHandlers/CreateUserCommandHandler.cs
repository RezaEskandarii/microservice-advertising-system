using AdvertisingSystem.Identity.Application.Interfaces;
using AdvertisingSystem.Identity.Application.UseCases.Commands;
using AdvertisingSystem.Identity.Application.UseCases.Queries.Dtos;
using AdvertisingSystem.Identity.Domain.DomainEvents;
using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Shared.Enums;
using AutoMapper;
using MediatR;

namespace AdvertisingSystem.Identity.Application.Handlers.CommandHandlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, GetUser>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IMapper mapper, IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<GetUser> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var appUser = AppUser.CreateNew(
            new(command.FirstName), new(command.LastName),
            new(command.PhoneNumber), new(command.Email),
            command.Address, UserStatuses.Enable
        );

        appUser.UserName = appUser.Email.Value;

        var result = await _userService.CreateAsync(appUser, command.Password);
        appUser.AddDomainEvent(new UserCreatedEvent(appUser, $"user created with username: {appUser.UserName}"));

        return _mapper.Map<GetUser>(result);
    }
}