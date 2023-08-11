using AdvertisingSystem.UserManagement.Api.ViewModels;
using AdvertisingSystem.UserManagement.Contract.Dtos.User;
using AdvertisingSystem.UserManagement.Contract.Interfaces;
using AdvertisingSystem.UserManagement.Shared.Constants;
using AdvertisingSystem.UserManagement.Shared.Enums;
using AdvertisingSystem.UserManagement.Shared.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.UserManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserAppService _userService;

    public UserController(IUserAppService userService)
    {
        _userService = userService;
    }

    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUpAsync(CreateUserDto userDto)
    {
        userDto.Role = UserRoles.Client;
        var createdUser = await _userService.CreateAsync(userDto);
        var apiResponse = new ApiResponse { Item = createdUser };
        return Ok(apiResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateUserDto userDto)
    {
        var createdUser = await _userService.CreateAsync(userDto);
        var apiResponse = new ApiResponse { Item = createdUser };
        return Ok(apiResponse);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, UpdateUserDto userDto)
    {
        var updatedUser = await _userService.UpdateAsync(id, userDto);
        var apiResponse = new ApiResponse { Item = updatedUser };
        return Ok(apiResponse);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FindAsync(string id)
    {
        var user = await _userService.FindAsync(id);
        if (user == null)
            return NotFound();

        var apiResponse = new ApiResponse { Item = user };
        return Ok(apiResponse);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery] FindUserFilter userFilter)
    {
        var paginatedResult = await _userService.GetPaginatedAsync(userFilter);
        var apiResponse = new ApiResponse { Item = paginatedResult };
        return Ok(apiResponse);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatusAsync(string id, UserStatuses status)
    {
        await _userService.ChangeStatusAsync(id, status);
        var apiResponse = new ApiResponse();
        return Ok(apiResponse);
    }

    [HttpGet("username/{username}")]
    public async Task<IActionResult> FindByUserNameAsync(string username)
    {
        var user = await _userService.FindByUserNameAsync(username);
        if (user == null)
            return NotFound();

        var apiResponse = new ApiResponse { Item = user };
        return Ok(apiResponse);
    }
}