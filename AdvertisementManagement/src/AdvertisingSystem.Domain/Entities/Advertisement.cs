using AdvertisingSystem.Domain.ValueObjects;

namespace AdvertisingSystem.Domain.Entities;

public class Advertisement : AggregateRoot
{
    private Advertisement()
    {
    }

    public string Title { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public CreateDate CreatedAt { get; set; }
    public UpdateDate UpdatedAt { get; set; }
    public ExpiryDate ExpiresAt { get; set; }
    public Address Address { get; set; }
    public int CategoryId { get; set; }
    public static Advertisement CreateNew(string title, string userId, string description, decimal price,
        CreateDate createdAt, UpdateDate updatedAt, ExpiryDate expiresAt,Address address,int categoryId)
    {
        return new Advertisement()
        {
            Title = title,
            UserId = userId,
            Description = description,
            Price = price,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            ExpiresAt = expiresAt,
            Address = address,
            CategoryId = categoryId
        };
    }
}