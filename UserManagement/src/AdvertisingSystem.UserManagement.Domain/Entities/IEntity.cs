using AdvertisingSystem.UserManagement.Domain.ValueObjects;

namespace AdvertisingSystem.UserManagement.Domain.Entities;

public interface IEntity<T>
{
    public T Id { get; set; }
    public CreatedAt CreatedAt { get; set; }
    public UpdatedAt UpdatedAt { get; set; }
}