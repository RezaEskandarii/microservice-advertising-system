using AdvertisingSystem.UserManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.UserManagement.Infrastructure.Persistence.ValueConverters;

public class CreatedAtConverter : ValueConverter<CreatedAt, DateTime>
{
    public CreatedAtConverter() : base(
        c => c.Value,
        date => new CreatedAt(date))
    {
    }
}