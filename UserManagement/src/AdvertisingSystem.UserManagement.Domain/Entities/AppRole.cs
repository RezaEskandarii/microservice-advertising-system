using Microsoft.AspNetCore.Identity;

namespace AdvertisingSystem.UserManagement.Domain.Entities;

public class AppRole : IdentityRole
{
    public string DisplayName { get; set; }
}