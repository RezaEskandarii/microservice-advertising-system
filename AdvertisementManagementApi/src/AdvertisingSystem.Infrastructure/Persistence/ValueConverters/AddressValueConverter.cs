using AdvertisingSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace AdvertisingSystem.Infrastructure.Persistence.ValueConverters;

public class AddressValueConverter : ValueConverter<Address, string>
{
    public AddressValueConverter() : base(
        address => JsonConvert.SerializeObject(address),
        json => JsonConvert.DeserializeObject<Address>(json))
    {
    }
}