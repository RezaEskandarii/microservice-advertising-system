namespace AdvertisingSystem.Identity.Domain.ValueObjects;

public class FirstName
{
    public readonly string Value;

    public FirstName(string value)
    {
        Value = value;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (FirstName)obj;
        return Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(FirstName firstName1, FirstName firstName2)
    {
        return firstName1.Equals(firstName2) || firstName1.Value == firstName2.Value;
    }

    public static bool operator !=(FirstName firstName1, FirstName firstName2)
    {
        return !(firstName1 == firstName2);
    }
}