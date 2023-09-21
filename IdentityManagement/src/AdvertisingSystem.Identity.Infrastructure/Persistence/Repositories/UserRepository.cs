using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Infrastructure.Persistence.Context;
using AdvertisingSystem.Identity.Shared;
using AdvertisingSystem.Identity.Shared.ExtensionMethods;
using AdvertisingSystem.Identity.Shared.Filters;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingSystem.Identity.Infrastructure.Persistence.Repositories;

[Obsolete("this class is deprecated, use from UserService in application layer")]
public class UserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<AppUser> CreateAsync(AppUser user, string password)
    {
        user.PasswordHash = password;
        var result = await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return result.Entity;
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
    
    public async Task<AppUser?> FindByUserNameAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }
}