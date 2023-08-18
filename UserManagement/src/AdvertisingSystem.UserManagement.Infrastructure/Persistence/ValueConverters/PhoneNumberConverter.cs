using AdvertisingSystem.Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.Identity.Infrastructure.Persistence.ValueConverters;

public class PhoneNumberConverter : ValueConverter<PhoneNumber, string>
{
    public PhoneNumberConverter() : base(
        c => c.Value,
        str => new PhoneNumber(str))
    {
    }
}