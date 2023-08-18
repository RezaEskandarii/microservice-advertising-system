using AdvertisingSystem.UserManagement.Domain.ValueObjects;
using AdvertisingSystem.UserManagement.Shared.Enums;

namespace AdvertisingSystem.UserManagement.Domain.Entities;

public class AppUser : UserAggregatedRoot
{
    private AppUser()
    {
    }

    public FirstName FirstName { get; set; }
    public LastName LastName { get; set; }
    public PhoneNumber CellNumber { get; set; }
    public Address? Address { get; set; }
    public UserStatuses Status { get; set; }

    public static AppUser CreateNew(FirstName firstName, LastName lastName, PhoneNumber cellNumber, Address? address,
        UserStatuses status)
    {
        return new AppUser()
        {
            FirstName = firstName,
            LastName = lastName,
            CellNumber = cellNumber,
            Address = address,
            Status = status,
        };
    }
}