using AdvertisingSystem.UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvertisingSystem.UserManagement.Infrastructure.EntityTypeConfigurations;

public class UserConfiguration: IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        throw new NotImplementedException();
    }
}