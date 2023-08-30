using AdvertisingSystem.Application.UseCases.Queries;
using AdvertisingSystem.Domain.ValueObjects;
using MediatR;
using Newtonsoft.Json;

namespace AdvertisingSystem.Application.UseCases.Commands;

public class UpdateAdvertisementCommand:IRequest<GetAdvertisement>
{
    public long AdvertsiementId { get; set; }
    public string Title { get; set; }
    [JsonIgnore]
    public string? UserId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Address? Address { get; set; }
    public int CategoryId { get; set; }
    public ICollection<ThumbnailFileModel> Thumbnails { get; set; } = new List<ThumbnailFileModel>();
    public string[]? Tags { get; set; }
}