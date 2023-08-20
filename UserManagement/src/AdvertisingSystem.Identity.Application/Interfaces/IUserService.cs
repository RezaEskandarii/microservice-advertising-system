using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Shared;
using AdvertisingSystem.Identity.Shared.Enums;
using AdvertisingSystem.Identity.Shared.Filters;

namespace AdvertisingSystem.Identity.Application.Interfaces;

public interface IUserService
{
    Task<AppUser> CreateAsync(AppUser user, string password);
    Task AddToRoleAsync(string username, string roleName);
    Task<AppUser?> UpdateAsync(string id, AppUser user);
    Task<AppUser?> FindAsync(string id);
    Task<bool> IsInRoleAsync(string username, string roleName);
    Task<PaginatedResult<AppUser>> GetPaginatedAsync(FindUserFilter userFilter);
    Task ChangeStatusAsync(string id, UserStatuses status);
    Task<AppUser?> FindByUserNameAsync(string username);
}