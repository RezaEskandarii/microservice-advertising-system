using AdvertisingSystem.UserManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using AdvertisingSystem.UserManagement.Shared.Enums;

namespace AdvertisingSystem.UserManagement.Domain.Entities;

public class AppUser : UserAggregatedRoot
{
    public FirstName FirstName { get; set; }
    public LastName LastName { get; set; }
    public PhoneNumber CellNumber { get; set; }
    public Address? Address { get; set; }
    public UserStatuses Status { get; set; }
}