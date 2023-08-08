using AdvertisingSystem.UserManagement.Contract.Dtos.User;
using AdvertisingSystem.UserManagement.Contract.Interfaces;
using AdvertisingSystem.UserManagement.Shared.Enums;
using AdvertisingSystem.UserManagement.Shared.Filters;

namespace AdvertisingSystem.UserManagement.Application.Services;

public class UserAppService : IUserAppService
{
    public Task<GetUserDto> CreateAsync(CreateUserDto userDto)
    {
        throw new NotImplementedException();
    }

    public Task<GetUserDto> UpdateAsync(string id, UpdateUserDto userDto)
    {
        throw new NotImplementedException();
    }

    public Task<GetUserDto> FindAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<GetUserDto> GetPaginatedAsync(FindUserFilter userFilter)
    {
        throw new NotImplementedException();
    }

    public Task ChangeStatusAsync(string id, UserStatuses status)
    {
        throw new NotImplementedException();
    }
}