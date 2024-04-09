using AdvertisingSystem.Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace AdvertisingSystem.Identity.Infrastructure.Persistence.ValueConverters;

public class AddressConverter : ValueConverter<Address, string>
{
    public AddressConverter() : base(
        address => JsonConvert.SerializeObject(address),
        json => JsonConvert.DeserializeObject<Address>(json) ?? new Address("", "", "", "", ""))
    {
    }
}