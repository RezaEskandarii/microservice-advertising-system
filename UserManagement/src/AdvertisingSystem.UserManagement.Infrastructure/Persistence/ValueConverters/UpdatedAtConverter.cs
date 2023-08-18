using AdvertisingSystem.Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.Identity.Infrastructure.Persistence.ValueConverters;

public class UpdatedAtConverter : ValueConverter<UpdatedAt, DateTime>
{
    public UpdatedAtConverter() : base(
        c => c.Value,
        date => new UpdatedAt(date))
    {
    }
}