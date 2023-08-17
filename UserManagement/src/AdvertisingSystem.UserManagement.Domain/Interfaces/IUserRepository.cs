using AdvertisingSystem.UserManagement.Domain.Entities;
using AdvertisingSystem.UserManagement.Shared;
using AdvertisingSystem.UserManagement.Shared.Enums;
using AdvertisingSystem.UserManagement.Shared.Filters;

namespace AdvertisingSystem.UserManagement.Domain.Interfaces;

public interface IUserRepository
{
    Task<AppUser> CreateAsync(AppUser user);
    Task<AppUser?> UpdateAsync(string id, AppUser user);
    Task<AppUser?> FindAsync(string id);
    Task<bool> IsInRoleAsync(string username, string roleName);
    Task<PaginatedResult<AppUser>> GetPaginatedAsync(FindUserFilter userFilter);
    Task ChangeStatusAsync(string id, UserStatuses status);
    Task<AppUser?> FindByUserNameAsync(string username);
}