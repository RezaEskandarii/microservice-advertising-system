using AdvertisingSystem.UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvertisingSystem.UserManagement.Infrastructure.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(x => x.Address).HasMaxLength(300);
        builder.Property(x => x.FirstName).HasMaxLength(20);
        builder.Property(x => x.LastName).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(30);
        builder.Property(x => x.PhoneNumber).HasMaxLength(15);
        builder.Property(x => x.CellNumber).HasMaxLength(15);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }
}