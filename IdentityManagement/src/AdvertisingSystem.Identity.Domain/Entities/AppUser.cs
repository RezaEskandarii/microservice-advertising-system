using AdvertisingSystem.Identity.Domain.ValueObjects;
using AdvertisingSystem.Identity.Shared.Enums;

namespace AdvertisingSystem.Identity.Domain.Entities;

public class AppUser : UserAggregatedRoot
{
    private AppUser()
    {
    }

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

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public PhoneNumber CellNumber { get; private set; }
    public Address? Address { get; private set; }
    public UserStatuses Status { get; private set; }
    public Email Email { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }


    public void SetRefreshTokenExpiresAt(DateTime expiresAt)
    {
        this.RefreshTokenExpiresAt = expiresAt;
    }

    public void SetRefreshToken(string refreshToken)
    {
        this.RefreshToken = refreshToken;
    }

    public void SetStatus(UserStatuses status)
    {
        this.Status = status;
    }
}