namespace AdvertisingSystem.Identity.Domain.ValueObjects;

public class PhoneNumber
{
    public readonly string? Value;

    public PhoneNumber(string? value)
    {
        Value = value;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (PhoneNumber)obj;
        return Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(PhoneNumber phoneNumber1, PhoneNumber phoneNumber2)
    {
        if (phoneNumber1 is null || phoneNumber2 is null)
        {
            return false;
        }
        
        return phoneNumber1.Equals(phoneNumber2) || phoneNumber1.Value == phoneNumber2.Value;

        return true;
    }

    public static bool operator !=(PhoneNumber phoneNumber1, PhoneNumber phoneNumber2)
    {
        return !(phoneNumber1 == phoneNumber2);
    }
}