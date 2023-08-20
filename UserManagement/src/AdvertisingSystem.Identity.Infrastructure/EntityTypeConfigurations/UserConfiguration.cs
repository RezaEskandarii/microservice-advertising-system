using AdvertisingSystem.Identity.Domain.Entities;
using AdvertisingSystem.Identity.Infrastructure.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvertisingSystem.Identity.Infrastructure.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(x => x.Address).HasConversion(new AddressConverter()).HasColumnType("jsonb");
        builder.Property(x => x.FirstName).HasConversion(new FirstNameConverter()).HasMaxLength(20);
        builder.Property(x => x.LastName).HasConversion(new LastNameConverter()).HasMaxLength(20);
        builder.Property(x => x.Email).HasConversion(new EmailConverter()).HasMaxLength(30);
        builder.Property(x => x.CellNumber).HasConversion(new PhoneNumberConverter()).HasMaxLength(14);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }
}