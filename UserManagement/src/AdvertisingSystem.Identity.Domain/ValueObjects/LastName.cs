namespace AdvertisingSystem.Identity.Domain.ValueObjects;

public class LastName
{
    public readonly string Value;

    public LastName(string value)
    {
        Value = value;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (LastName)obj;
        return Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(LastName lastName1, LastName lastName2)
    {
        return lastName1.Equals(lastName2) || lastName1.Value == lastName2.Value;
    }

    public static bool operator !=(LastName lastName1, LastName lastName2)
    {
        return !(lastName1 == lastName2);
    }
}