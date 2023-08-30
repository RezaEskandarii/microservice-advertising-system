using AdvertisingSystem.Domain.ValueObjects;

namespace AdvertisingSystem.Application.ViewModels;

public class GetAdvertisementViewModel
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public decimal? Price { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Address? Address { get; set; }
    public int CategoryId { get; set; }
    public ICollection<string> Thumbnails { get; set; } = new List<string>();
    public string[]? Tags { get; set; }
}