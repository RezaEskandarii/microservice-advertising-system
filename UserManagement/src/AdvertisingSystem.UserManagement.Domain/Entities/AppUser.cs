using Microsoft.AspNetCore.Identity;
using AdvertisingSystem.UserManagement.Shared.Enums;

namespace AdvertisingSystem.UserManagement.Domain.Entities;

public class AppUser : IdentityUser<string>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellNumber { get; set; }
    public string Address { get; set; }
    public UserStatuses Status { get; set; }
}