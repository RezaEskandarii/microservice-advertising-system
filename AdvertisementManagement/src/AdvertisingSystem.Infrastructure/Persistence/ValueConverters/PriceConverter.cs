using AdvertisingSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.Infrastructure.Persistence.ValueConverters;

public class PriceConverter : ValueConverter<Price, decimal>
{
    public PriceConverter() : base(
        c => c.Amount,
        date => new Price(date, "USD"))
    {
    }
}