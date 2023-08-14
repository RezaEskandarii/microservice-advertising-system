using AdvertisingSystem.Domain.ValueObjects;
using MediatR;

namespace AdvertisingSystem.Application.UseCases.Commands;

public class UpdateAdvertisementCommand : IRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Price Price { get; set; }
    public UpdateDate UpdatedAt { get; set; }
    public ExpiryDate ExpiresAt { get; set; }
    public Address Address { get; set; }
}