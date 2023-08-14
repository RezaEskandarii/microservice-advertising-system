using AdvertisingSystem.Domain.ValueObjects;
using MediatR;

namespace AdvertisingSystem.Application.UseCases.Commands;

public class CreateAdvertisementCommand : IRequest
{
    public string Title { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public Price Price { get; set; }
    public CreateDate CreatedAt { get; set; }
    public UpdateDate UpdatedAt { get; set; }
    public ExpiryDate ExpiresAt { get; set; }
    public Address Address { get; set; }
}