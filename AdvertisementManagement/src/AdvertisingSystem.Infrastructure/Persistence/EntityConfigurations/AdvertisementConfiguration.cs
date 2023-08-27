using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Infrastructure.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvertisingSystem.Infrastructure.Persistence.EntityConfigurations;

public class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Address).HasConversion(new AddressValueConverter());
        builder.Property(x => x.CreatedAt).HasConversion(new CreatedAtConverter());
        builder.Property(x => x.UpdatedAt).HasConversion(new UpdatedAtConverter());
        builder.Property(x => x.ExpiresAt).HasConversion(new ExpiryDateConverter());
        builder.Property(x => x.Tags).HasColumnType("text[]");
        builder.Property(x => x.Thumbnails).HasColumnType("text[]");

        builder.ToTable("Advertisements");
    }
}