using AdvertisingSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.Infrastructure.Persistence.ValueConverters;

public class UpdatedAtConverter : ValueConverter<UpdateDate, DateTime>
{
    public UpdatedAtConverter() : base(
        c => c.Value,
        date => new UpdateDate(date))
    {
    }
}