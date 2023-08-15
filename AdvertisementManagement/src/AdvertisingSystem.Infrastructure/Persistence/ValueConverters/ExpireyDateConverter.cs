using AdvertisingSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.Infrastructure.Persistence.ValueConverters;

public class ExpiryDateConverter : ValueConverter<ExpiryDate, DateTime>
{
    public ExpiryDateConverter() : base(
        c => c.Value,
        date => new ExpiryDate(date))
    {
    }
}