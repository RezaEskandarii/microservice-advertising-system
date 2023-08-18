using AdvertisingSystem.UserManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.UserManagement.Infrastructure.Persistence.ValueConverters;

public class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter() : base(
        c => c.Value,
        str => new Email(str))
    {
    }
}