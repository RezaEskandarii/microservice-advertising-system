using AdvertisingSystem.UserManagement.Contract.Dtos.User;
using AdvertisingSystem.UserManagement.Shared;
using AdvertisingSystem.UserManagement.Shared.Enums;
using AdvertisingSystem.UserManagement.Shared.Filters;

namespace AdvertisingSystem.UserManagement.Contract.Interfaces;

public interface IUserAppService
{
    Task<GetUserDto> CreateAsync(CreateUserDto userDto);
    Task<GetUserDto> UpdateAsync(string id, UpdateUserDto userDto);
    Task<GetUserDto> FindAsync(string id);
    Task<PaginatedResult<GetUserDto>> GetPaginatedAsync(FindUserFilter userFilter);
    Task ChangeStatusAsync(string id, UserStatuses status);
    Task<GetUserDto> FindByUserNameAsync(string username);
}