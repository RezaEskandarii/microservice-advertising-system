using AdvertisingSystem.Identity.Application.Interfaces;
using AdvertisingSystem.Identity.Application.UseCases.Commands;
using AdvertisingSystem.Identity.Application.UseCases.Queries.Dtos;
using AutoMapper;
using MediatR;

namespace AdvertisingSystem.Identity.Application.Handlers.CommandHandlers;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, GetUser>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<GetUser> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(command.Id, command);
        return _mapper.Map<GetUser>(result);
    }
}