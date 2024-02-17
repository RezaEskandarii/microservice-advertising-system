using AdvertisingSystem.Application.ViewModels;
using MediatR;

namespace AdvertisingSystem.Application.UseCases.Queries;

public record GetAdvertisementQuery(long Id) : IRequest<GetAdvertisementViewModel>;