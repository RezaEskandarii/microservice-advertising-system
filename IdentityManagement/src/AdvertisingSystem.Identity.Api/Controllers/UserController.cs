using AdvertisingSystem.Identity.Api.ViewModels;
using AdvertisingSystem.Identity.Application.UseCases.Commands;
using AdvertisingSystem.Identity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.SuperAdmin)]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost("SignUp")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUpAsync(CreateUserCommand command)
    {
        command.Role = UserRoles.Customer;
        var createdUser = await _mediator.Send(command);
        return Ok(new ApiResponse { ResponseObject = createdUser });
    }

    [HttpPost("SignIn")]
    [AllowAnonymous]
    public async Task<IActionResult> SignInAsync(LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new ApiResponse { ResponseObject = result });
    }


    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateUserCommand command)
    {
        var createdUser = await _mediator.Send(command);
        return Ok(new ApiResponse { ResponseObject = createdUser });
    }

    [HttpPut("ChangePassword/{userId:guid}")]
    public async Task<IActionResult> UpdatePasswordAsync([FromRoute] Guid userId, UpdatePasswordCommand command)
    {
        command.userId = userId.ToString();
        await _mediator.Send(command);
        return Ok(new ApiResponse());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, UpdateUserCommand command)
    {
        command.Id = id;
        var updatedUser = await _mediator.Send(command);
        return Ok(new ApiResponse { ResponseObject = updatedUser });
    }


    // [HttpGet("{id}")]
    // public async Task<IActionResult> FindAsync(string id)
    // {
    //     var user = await _userService.FindAsync(id);
    //     if (user == null)
    //         return NotFound();
    //     return Ok(new ApiResponse { Item = user });
    // }

    // [HttpGet]
    // public async Task<IActionResult> GetPaginatedAsync([FromQuery] FindUserFilter userFilter)
    // {
    //     var paginatedResult = await _userService.GetPaginatedAsync(userFilter);
    //     return Ok(new ApiResponse { Item = paginatedResult });
    // }
    //
    // [HttpPut("{id}/status")]
    // public async Task<IActionResult> ChangeStatusAsync(string id, ChangeS)
    // {
    //     await _userService.ChangeStatusAsync(id, status);
    //     return Ok(new ApiResponse());
    // }

    // [HttpGet("username/{username}")]
    // public async Task<IActionResult> FindByUserNameAsync(string username)
    // {
    //     var user = await _userService.FindByUserNameAsync(username);
    //     if (user == null)
    //         return NotFound();
    //
    //     return Ok(new ApiResponse { Item = user });
    // }
}