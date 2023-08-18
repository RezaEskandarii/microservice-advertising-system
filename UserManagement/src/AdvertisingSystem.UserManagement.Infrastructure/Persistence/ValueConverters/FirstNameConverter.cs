using AdvertisingSystem.Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.Identity.Infrastructure.Persistence.ValueConverters;

public class FirstNameConverter : ValueConverter<FirstName, string>
{
    public FirstNameConverter() : base(
        c => c.Value,
        str => new FirstName(str))
    {
    }
}