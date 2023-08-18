namespace AdvertisingSystem.UserManagement.Domain.ValueObjects;

public class UpdatedAt : IEquatable<UpdatedAt>
{
    public DateTime Value { get; }

    public UpdatedAt(DateTime value)
    {
        Value = value;
    }

    public static bool operator ==(UpdatedAt left, UpdatedAt right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(UpdatedAt left, UpdatedAt right)
    {
        return !(left == right);
    }

    public bool Equals(UpdatedAt other)
    {
        if (other == null)
            return false;

        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((UpdatedAt)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}