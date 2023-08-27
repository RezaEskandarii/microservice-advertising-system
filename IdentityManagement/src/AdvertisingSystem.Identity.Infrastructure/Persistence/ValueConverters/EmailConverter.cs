using AdvertisingSystem.Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdvertisingSystem.Identity.Infrastructure.Persistence.ValueConverters;

public class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter() : base(
        c => c.Value,
        str => new Email(str))
    {
    }
}