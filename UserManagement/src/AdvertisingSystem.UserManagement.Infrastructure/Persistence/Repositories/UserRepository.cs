using AdvertisingSystem.UserManagement.Domain.Entities;
using AdvertisingSystem.UserManagement.Domain.Interfaces;
using AdvertisingSystem.UserManagement.Infrastructure.Persistence.Context;
using AdvertisingSystem.UserManagement.Shared;
using AdvertisingSystem.UserManagement.Shared.Enums;
using AdvertisingSystem.UserManagement.Shared.ExtensionMethods;
using AdvertisingSystem.UserManagement.Shared.Filters;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingSystem.UserManagement.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<AppUser> CreateAsync(AppUser user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<AppUser?> UpdateAsync(string id, AppUser user)
    {
        var existingUser = await _dbContext.Users.FindAsync(id);
        if (existingUser == null) return existingUser;
        _mapper.Map(user, existingUser);
        await _dbContext.SaveChangesAsync();

        return existingUser;
    }

    public async Task<AppUser?> FindAsync(string id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string roleName)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            // User not found
            return false;
        }

        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            // Role not found
            return false;
        }

        var userRole =
            await _dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);
        return userRole != null;
    }

    public async Task<PaginatedResult<AppUser>> GetPaginatedAsync(FindUserFilter userFilter)
    {
        var query = _dbContext.Users.AsQueryable();

        // Apply filtering
        if (!string.IsNullOrEmpty(userFilter.Id))
        {
            query = query.Where(u => u.FirstName.Value.Contains(userFilter.FirstName));
        }

        var totalRecords = await query.CountAsync();
        return new PaginatedResult<AppUser>()
        {
            Items = await query.Paginate(userFilter).ToListAsync(),
            PageNumber = userFilter.PageNumber,
            PageSize = userFilter.PageSize,
            TotalCount = totalRecords
        };
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