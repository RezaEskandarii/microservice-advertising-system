using AdvertisingSystem.UserManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.UserManagement.Infrastructure.Persistence.ValueConverters;

public class LastNameConverter : ValueConverter<LastName, string>
{
    public LastNameConverter() : base(
        c => c.Value,
        str => new LastName(str))
    {
    }
}