using AdvertisingSystem.Identity.Application.UseCases.Commands;
using AdvertisingSystem.Identity.Application.UseCases.Queries;
using AdvertisingSystem.Identity.Domain.DomainEvents;
using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Domain.Interfaces;
using AdvertisingSystem.Identity.Shared.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AdvertisingSystem.Identity.Application.Handlers.CommandHandlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, GetUser>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IMapper mapper, UserManager<AppUser> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<GetUser> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var appUser = AppUser.CreateNew(new(command.FirstName), new(command.LastName),
            new(command.PhoneNumber), new(command.Email), command.Address, UserStatuses.Enable);

        appUser.UserName = appUser.Email.Value;
        var result = await _userManager.CreateAsync(appUser);
        appUser.AddDomainEvent(new UserCreatedEvent(appUser, $"use created with username: {appUser.UserName}"));

        return _mapper.Map<GetUser>(result);
    }
}