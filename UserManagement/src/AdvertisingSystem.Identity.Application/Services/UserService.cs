using AdvertisingSystem.Identity.Application.Interfaces;
using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Domain.ValueObjects;
using AdvertisingSystem.Identity.Shared;
using AdvertisingSystem.Identity.Shared.Constants;
using AdvertisingSystem.Identity.Shared.Enums;
using AdvertisingSystem.Identity.Shared.Exceptions;
using AdvertisingSystem.Identity.Shared.ExtensionMethods;
using AdvertisingSystem.Identity.Shared.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingSystem.Identity.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly RoleManager<AppRole> _roleManager;

    public UserService(UserManager<AppUser> userManager, IMapper mapper, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public async Task<AppUser> CreateAsync(AppUser user, string password)
    {
        await ThrowIfEmailDuplicatedAsync(user.Email);
        await ThrowIfPhoneNumberDuplicatedAsync(new PhoneNumber(user.PhoneNumber));

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddPasswordAsync(user, password);
            await AddToRoleAsync(user.Email.Value, UserRoles.Client);
            return user;
        }
        else
        {
            // Handle creation failure
            throw new Exception(string.Join("\n", result.Errors));
        }
    }

    public async Task AddToRoleAsync(string username, string roleName)
    {
        var exists = await _roleManager.RoleExistsAsync(roleName);
        if (!exists)
            await _roleManager.CreateAsync(new AppRole()
            {
                Name = roleName,
                DisplayName = roleName
            });
        var user = await _userManager.FindByNameAsync(username);
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<AppUser?> UpdateAsync(string id, AppUser user)
    {
        var existingUser = await _userManager.FindByIdAsync(id);
        if (existingUser != null)
        {
            // Update user properties
            _mapper.Map(user, existingUser);
            var result = await _userManager.UpdateAsync(existingUser);
            if (result.Succeeded)
            {
                return existingUser;
            }
            else
            {
                // Handle update failure
                ThrowIdentityExceptions(result);
            }
        }

        return null;
    }

    public async Task<AppUser?> FindAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<bool> IsInRoleAsync(string username, string roleName)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user != null)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        return false;
    }

    public async Task<PaginatedResult<AppUser>> GetPaginatedAsync(FindUserFilter userFilter)
    {
        var query = _userManager.Users.AsQueryable();
        query = GetFilteredQuery(query, userFilter);
        var totalCount = await query.CountAsync();

        return new PaginatedResult<AppUser>(userFilter)
        {
            TotalCount = totalCount,
            Items = await query.Paginate(userFilter).ToListAsync()
        };
    }


    public async Task ChangeStatusAsync(string id, UserStatuses status)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            // Update user status
            user.Status = status;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                // Handle update failure
                ThrowIdentityExceptions(result);
            }
        }
    }

    public async Task<AppUser?> FindByUserNameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    #region Private

    private IQueryable<AppUser> GetFilteredQuery(IQueryable<AppUser> query, FindUserFilter userFilter)
    {
        return query;
    }

    private void ThrowIdentityExceptions(IdentityResult? result)
    {
        if (result is not null && result.Errors.Any())
        {
            throw new Exception(string.Join("\n", result.Errors));
        }
    }

    private async Task ThrowIfEmailDuplicatedAsync(Email email)
    {
        var appUser = await _userManager.FindByEmailAsync(email.Value);
        if (appUser != null)
        {
            throw new DuplicatedUserException(email.Value);
        }
    }

    private async Task ThrowIfPhoneNumberDuplicatedAsync(PhoneNumber phoneNumber)
    {
        var appUser = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber.Value);
        if (appUser != null)
        {
            throw new DuplicatedUserException(phoneNumber.Value);
        }
    }

    #endregion
}