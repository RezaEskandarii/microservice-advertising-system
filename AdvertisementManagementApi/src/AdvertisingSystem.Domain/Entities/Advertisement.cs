using AdvertisingSystem.Domain.ValueObjects;

namespace AdvertisingSystem.Domain.Entities;

public class Advertisement : AggregateRoot
{
    private Advertisement()
    {
    }

    public static Advertisement CreateNew(string title, string userId, string description, Price price,
        CreateDate createdAt, UpdateDate updatedAt, ExpiryDate expiresAt, Address? address, int categoryId,
        string[]? tags, List<Property>? properties, int? locationId)
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
            CategoryId = categoryId,
            Tags = tags,
            Properties = properties,
            LocationId = locationId
        };
    }

    public string Title { get; private set; }
    public string UserId { get; private set; }
    public string Description { get; private set; }
    public Price? Price { get; private set; }
    public CreateDate CreatedAt { get; private set; }
    public UpdateDate UpdatedAt { get; private set; }
    public ExpiryDate ExpiresAt { get; private set; }
    public Address? Address { get; private set; }
    public int CategoryId { get; private set; }
    public string[]? Tags { get; private set; }
    public string[]? Thumbnails { get; private set; } = new[] { "" };
    public List<Property>? Properties { get; private set; } = new List<Property>();
    public int? LocationId { get; private set; }
    public bool IsSyncedInReadDb { get; private set; }
    public long ViewCount { get; private set; }

    public void UpdateTitle(string newTitle)
    {
        Title = newTitle;
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
    }

    public void UpdatePrice(Price newPrice)
    {
        Price = newPrice;
    }

    public void UpdateExpiresAt(ExpiryDate newExpiresAt)
    {
        ExpiresAt = newExpiresAt;
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress;
    }

    public void UpdateCategoryId(int newCategoryId)
    {
        CategoryId = newCategoryId;
    }

    public void UpdateIsSyncedInReadDb(bool isSyncedInReadDb)
    {
        IsSyncedInReadDb = isSyncedInReadDb;
    }

    public void UpdateTags(string[]? newTags)
    {
        this.Tags = newTags;
    }

    public void UpdateThumbnails(string[] newThumbnails)
    {
        Thumbnails = newThumbnails;
    }

    public void UpdateViewCount(long viewCount)
    {
        ViewCount = viewCount;
    }
}