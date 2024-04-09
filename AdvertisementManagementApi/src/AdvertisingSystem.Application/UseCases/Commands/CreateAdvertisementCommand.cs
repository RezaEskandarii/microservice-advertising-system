using AdvertisingSystem.Application.ViewModels;
using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Domain.ValueObjects;
using MediatR;
using Newtonsoft.Json;

namespace AdvertisingSystem.Application.UseCases.Commands;

public class CreateAdvertisementCommand : IRequest<GetAdvertisementViewModel>
{
    public string Title { get; set; }
    [JsonIgnore] public string? UserId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Address? Address { get; set; }
    public int CategoryId { get; set; }
    public ICollection<ThumbnailFileViewModel>? Thumbnails { get; set; } = new List<ThumbnailFileViewModel>();
    public string[]? Tags { get; set; }
    public List<Property>? Properties { get; set; } = new();
    public int? LocationId { get; set; }
}