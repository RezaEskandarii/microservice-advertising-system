using AdvertisingSystem.Identity.Application.Interfaces;
using AdvertisingSystem.Identity.Application.UseCases.Commands;
using MediatR;

namespace AdvertisingSystem.Identity.Application.Handlers.CommandHandlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserService _userService;

    public LoginCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        return await _userService.GenerateJwtAsync(command);
    }
}