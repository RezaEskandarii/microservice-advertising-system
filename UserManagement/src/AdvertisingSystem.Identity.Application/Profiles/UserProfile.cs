using AdvertisingSystem.Identity.Application.UseCases.Queries;
using AdvertisingSystem.Identity.Domain.Entities;
using AutoMapper;

namespace AdvertisingSystem.Identity.Application.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser, GetUser>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Value))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.CellNumber.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));
    }
}