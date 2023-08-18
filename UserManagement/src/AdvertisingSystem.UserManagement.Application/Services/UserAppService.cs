using AdvertisingSystem.UserManagement.Contract.Dtos.User;
using AdvertisingSystem.UserManagement.Contract.Interfaces;
using AdvertisingSystem.UserManagement.Domain.Entities;
using AdvertisingSystem.UserManagement.Infrastructure.Persistence.Context;
using AdvertisingSystem.UserManagement.Shared;
using AdvertisingSystem.UserManagement.Shared.Enums;
using AdvertisingSystem.UserManagement.Shared.Exceptions;
using AdvertisingSystem.UserManagement.Shared.ExtensionMethods;
using AdvertisingSystem.UserManagement.Shared.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingSystem.UserManagement.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public UserAppService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetUserDto> CreateAsync(CreateUserDto userDto)
    {
        await ThrowIfEmailDuplicatedAsync(userDto.Email);
        ThrowIfPhoneNumberDuplicated(userDto.PhoneNumber);
        ThrowIfCellNumberDuplicated(userDto.CellNumber);

        var appUser = _mapper.Map<AppUser>(userDto);
        appUser.UserName = userDto.Email;

        var result = await _userManager.CreateAsync(appUser);
        if (result.Succeeded)
        {
            await _userManager.AddPasswordAsync(appUser, userDto.Password);
            // Assign the user to the role
            await AddToRoleAsync(appUser, userDto.Role);
            return await FindByUserNameAsync(userDto.Email);
        }

        var errorMessage = string.Join("\n", result.Errors);
        throw new Exception(errorMessage);
    }

    public async Task<GetUserDto?> FindByUserNameAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return _mapper.Map<GetUserDto>(user);
    }

    public async Task<GetUserDto> UpdateAsync(string id, UpdateUserDto userDto)
    {
        var appUser = await _userManager.FindByIdAsync(id);
        _mapper.Map(userDto, appUser);
        await _userManager.UpdateAsync(appUser);

        return await FindAsync(id);
    }

    public async Task<GetUserDto?> FindAsync(string id)
    {
        var appUser = await _userManager.FindByIdAsync(id);
        return _mapper.Map<GetUserDto>(appUser);
    }

    public async Task<bool> IsInRoleAsync(string username, string roleName)
    {
        var appUser = await _userManager.FindByNameAsync(username);
        return await _userManager.IsInRoleAsync(appUser, roleName);
    }

    public async Task<PaginatedResult<GetUserDto>> GetPaginatedAsync(FindUserFilter userFilter)
    {
        var query = _context.Users.AsNoTracking().AsQueryable();
        query = GetFilteredQuery(query, userFilter);
        var totalRecords = await query.CountAsync();
        var users = await query.Paginate(userFilter).ToListAsync();

        return new PaginatedResult<GetUserDto>
        {
            Items = _mapper.Map<ICollection<GetUserDto>>(users),
            PageNumber = userFilter.PageNumber,
            PageSize = userFilter.PageSize,
            TotalCount = totalRecords
        };
    }


    public async Task ChangeStatusAsync(string id, UserStatuses status)
    {
        var appUser = await _userManager.FindByIdAsync(id);
        appUser.Status = status;
        await _userManager.UpdateAsync(appUser);
    }


    #region Private

    private IQueryable<AppUser> GetFilteredQuery(IQueryable<AppUser> query, FindUserFilter userFilter)
    {
        if (!string.IsNullOrWhiteSpace(userFilter.Id)) query = query.Where(x => x.Id == userFilter.Id);

        return query;
    }

    private async Task ThrowIfEmailDuplicatedAsync(string email)
    {
        var appUser = await _userManager.FindByNameAsync(email);
        if (appUser != null) throw new DuplicatedUserException(email);
    }

    private void ThrowIfPhoneNumberDuplicated(string cellNumber)
    {
        if (string.IsNullOrWhiteSpace(cellNumber)) return;
        var appUser = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == cellNumber);
        if (appUser != null) throw new DuplicatedUserException(cellNumber);
    }

    private void ThrowIfCellNumberDuplicated(string cellNumber)
    {
        if (string.IsNullOrWhiteSpace(cellNumber)) return;
        var appUser = _userManager.Users.FirstOrDefault(x => x.CellNumber == cellNumber);
        if (appUser != null) throw new DuplicatedUserException(cellNumber);
    }

    private async Task AddToRoleAsync(AppUser appUser, string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
            await _roleManager.CreateAsync(new AppRole { Name = roleName, DisplayName = roleName });
        await _userManager.AddToRoleAsync(appUser, roleName);
    }

    #endregion
}