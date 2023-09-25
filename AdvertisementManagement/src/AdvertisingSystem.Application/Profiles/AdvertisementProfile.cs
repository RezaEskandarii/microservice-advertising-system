using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Domain.Entities;
using AutoMapper;

namespace AdvertisingSystem.Application.Profiles;

public class AdvertisementProfile : Profile
{
    public AdvertisementProfile()
    {
        
        CreateMap<Property, KeyValuePair<string, string>>()
            .ConstructUsing(p => new KeyValuePair<string?, string?>(p.Name, p.Value));
        
        CreateMap<Advertisement, GetAdvertisementViewModel>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src!.Price!.Amount))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt.Value))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value))
            .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties));

        CreateMap<UpdateAdvertisementCommand, Advertisement>();
    }
}