using AdvertisingSystem.Application.UseCases.Commands;
using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Domain;
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

        CreateMap<PaginatedList<Advertisement>, PaginatedList<GetAdvertisementViewModel>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(src => src.TotalCount));
        
        CreateMap<UpdateAdvertisementCommand, Advertisement>();
        CreateMap<PaginatedList<Advertisement>, PaginatedList<GetAdvertisementViewModel>>();
    }
}