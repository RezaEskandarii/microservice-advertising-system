using AdvertisingSystem.UserManagement.Contract.Dtos.User;
using AdvertisingSystem.UserManagement.Domain.Entities;
using AutoMapper;

namespace AdvertisingSystem.UserManagement.Application.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, AppUser>();
        CreateMap<UpdateUserDto, AppUser>();
        CreateMap<AppUser, GetUserDto>();
    }
}