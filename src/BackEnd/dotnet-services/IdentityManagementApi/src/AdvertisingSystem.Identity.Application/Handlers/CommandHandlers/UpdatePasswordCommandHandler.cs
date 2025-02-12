using AdvertisingSystem.Identity.Application.Interfaces;
using AdvertisingSystem.Identity.Application.UseCases.Commands;
using MediatR;

namespace AdvertisingSystem.Identity.Application.Handlers.CommandHandlers;

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand>
{
    private readonly IUserService _userService;

    public UpdatePasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
    {
        await _userService.ChanePasswordAsync(command.userId, command.Password);
    }
}