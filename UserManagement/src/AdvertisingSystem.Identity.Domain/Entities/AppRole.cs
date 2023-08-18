using Microsoft.AspNetCore.Identity;

namespace AdvertisingSystem.Identity.Domain.Entities;

public class AppRole : IdentityRole
{
    public string DisplayName { get; set; }
}