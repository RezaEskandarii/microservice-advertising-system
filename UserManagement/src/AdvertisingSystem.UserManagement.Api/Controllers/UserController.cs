using AdvertisingSystem.UserManagement.Api.ViewModels;
using AdvertisingSystem.UserManagement.Contract.Dtos.User;
using AdvertisingSystem.UserManagement.Contract.Interfaces;
using AdvertisingSystem.UserManagement.Shared.Constants;
using AdvertisingSystem.UserManagement.Shared.Enums;
using AdvertisingSystem.UserManagement.Shared.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.UserManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.SuperAdmin)]
public class UserController : ControllerBase
{
    private readonly IUserAppService _userService;

    public UserController(IUserAppService userService)
    {
        _userService = userService;
    }

    [HttpPost("SignUp")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUpAsync(CreateUserDto userDto)
    {
        userDto.Role = UserRoles.Client;
        var createdUser = await _userService.CreateAsync(userDto);
        return Ok(new ApiResponse { Item = createdUser });
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateUserDto userDto)
    {
        var createdUser = await _userService.CreateAsync(userDto);
        return Ok(new ApiResponse { Item = createdUser });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, UpdateUserDto userDto)
    {
        var updatedUser = await _userService.UpdateAsync(id, userDto);
        return Ok(new ApiResponse { Item = updatedUser });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FindAsync(string id)
    {
        var user = await _userService.FindAsync(id);
        if (user == null)
            return NotFound();
        return Ok(new ApiResponse { Item = user });
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery] FindUserFilter userFilter)
    {
        var paginatedResult = await _userService.GetPaginatedAsync(userFilter);
        return Ok(new ApiResponse { Item = paginatedResult });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatusAsync(string id, UserStatuses status)
    {
        await _userService.ChangeStatusAsync(id, status);
        return Ok(new ApiResponse());
    }

    [HttpGet("username/{username}")]
    public async Task<IActionResult> FindByUserNameAsync(string username)
    {
        var user = await _userService.FindByUserNameAsync(username);
        if (user == null)
            return NotFound();

        return Ok(new ApiResponse { Item = user });
    }
}