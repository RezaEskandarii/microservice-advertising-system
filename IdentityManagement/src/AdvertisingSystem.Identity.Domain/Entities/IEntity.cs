using AdvertisingSystem.Identity.Domain.ValueObjects;

namespace AdvertisingSystem.Identity.Domain.Entities;

public interface IEntity<T>
{
    public T Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}