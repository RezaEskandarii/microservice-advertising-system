using AdvertisingSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace AdvertisingSystem.Infrastructure.Persistence.ValueConverters;

public class PriceConverter : ValueConverter<Price, string>
{
    public PriceConverter() : base(
        price => JsonConvert.SerializeObject(price),
        json => JsonConvert.DeserializeObject<Price>(json))
    {
    }
}