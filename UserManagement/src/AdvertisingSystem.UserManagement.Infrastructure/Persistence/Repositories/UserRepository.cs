using AdvertisingSystem.UserManagement.Domain.Entities;
using AdvertisingSystem.UserManagement.Domain.Interfaces;
using AdvertisingSystem.UserManagement.Infrastructure.Persistence.Context;
using AdvertisingSystem.UserManagement.Shared;
using AdvertisingSystem.UserManagement.Shared.Enums;
using AdvertisingSystem.UserManagement.Shared.Filters;

namespace AdvertisingSystem.UserManagement.Infrastructure.Persistence.Repositories;



public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppUser> CreateAsync(AppUser user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<AppUser> UpdateAsync(string id, AppUser user)
    {
        var existingUser = await _dbContext.Users.FindAsync(id);
        if (existingUser != null)
        {
            existingUser.FirstName = user.FirstName;
            existingUser.Email = user.Email;
            // Update other properties as needed
            await _dbContext.SaveChangesAsync();
        }
        return existingUser;
    }

    public async Task<AppUser?> FindAsync(string id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<bool> IsInRoleAsync(string username, string roleName)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            return false;
        }
        return user.Roles.Contains(roleName);
    }

    public async Task<PaginatedResult<AppUser>> GetPaginatedAsync(FindUserFilter userFilter)
    {
        var query = _dbContext.Users.AsQueryable();

        // Apply filtering
        if (!string.IsNullOrEmpty(userFilter.Name))
        {
            query = query.Where(u => u.Name.Contains(userFilter.Name));
        }

        // Apply sorting
        switch (userFilter.SortBy)
        {
            case SortBy.Name:
                query = query.OrderBy(u => u.Name);
                break;
            // Add more cases as needed
            default:
                query = query.OrderBy(u => u.UserName);
                break;
        }

        // Apply pagination
        var totalItems = await query.CountAsync();
        var users = await query.Skip(userFilter.PageNumber * userFilter.PageSize)
                               .Take(userFilter.PageSize)
                               .ToListAsync();

        return new PaginatedResult<AppUser>(users, totalItems);
    }

    public async Task ChangeStatusAsync(string id, UserStatuses status)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user != null)
        {
            user.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<AppUser?> FindByUserNameAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }
}