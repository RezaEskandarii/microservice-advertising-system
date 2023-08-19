using AdvertisingSystem.Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.Identity.Infrastructure.Persistence.ValueConverters;

public class CreatedAtConverter : ValueConverter<CreatedAt, DateTime>
{
    public CreatedAtConverter() : base(
        c => c.Value,
        date => new CreatedAt(date))
    {
    }
}