using AdvertisingSystem.UserManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.UserManagement.Infrastructure.Persistence.ValueConverters;

public class FirstNameConverter : ValueConverter<FirstName, string>
{
    public FirstNameConverter() : base(
        c => c.Value,
        str => new FirstName(str))
    {
    }
}