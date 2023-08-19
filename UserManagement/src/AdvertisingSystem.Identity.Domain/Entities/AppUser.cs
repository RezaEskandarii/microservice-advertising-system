using AdvertisingSystem.Identity.Domain.ValueObjects;
using AdvertisingSystem.Identity.Shared.Enums;

namespace AdvertisingSystem.Identity.Domain.Entities;

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
    public Email Email { get; set; }

    public static AppUser CreateNew(FirstName firstName, LastName lastName, PhoneNumber cellNumber, Email email,
        Address? address,
        UserStatuses status)
    {
        return new AppUser()
        {
            FirstName = firstName,
            LastName = lastName,
            CellNumber = cellNumber,
            Email = email,
            Address = address,
            Status = status,
        };
    }
}