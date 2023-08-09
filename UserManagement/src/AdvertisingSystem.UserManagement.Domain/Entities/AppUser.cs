using Microsoft.AspNetCore.Identity;

namespace AdvertisingSystem.UserManagement.Domain.Entities;

public class AppUser : IdentityUser<string>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellNumber { get; set; }
    public string Address { get; set; }
}