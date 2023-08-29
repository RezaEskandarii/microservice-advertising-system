using AdvertisingSystem.Application.UseCases.Queries;
using AdvertisingSystem.Domain.Entities;
using AutoMapper;

namespace AdvertisingSystem.Application.Profiles;

public class AdvertisementProfile : Profile
{
    public AdvertisementProfile()
    {
        CreateMap<Advertisement, GetAdvertisement>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt.Value))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value));
    }
}