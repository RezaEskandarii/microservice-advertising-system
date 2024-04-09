using AdvertisingSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace AdvertisingSystem.Infrastructure.Persistence.ValueConverters;

public class CreatedAtConverter : ValueConverter<CreateDate, DateTime>
{
    public CreatedAtConverter() : base(
        c => c.Value,
        date => new CreateDate(date))
    {
    }
}