using AdvertisingSystem.Domain.ValueObjects;
using MediatR;

namespace AdvertisingSystem.Application.UseCases.Commands;

public class CreateAdvertisementCommand : IRequest
{
    public string Title { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Address Address { get; set; }
    public int CategoryId { get; set; }
}