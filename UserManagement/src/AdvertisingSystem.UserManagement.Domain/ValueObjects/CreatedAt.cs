namespace AdvertisingSystem.UserManagement.Domain.ValueObjects;

using System;

public class CreatedAt : IEquatable<CreatedAt>
{
    public DateTime Value { get; }

    public CreatedAt(DateTime value)
    {
        if (value < DateTime.Now.AddDays(-1))
        {
            throw new Exception("");
        }

        Value = value;
    }

    public static bool operator ==(CreatedAt left, CreatedAt right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(CreatedAt left, CreatedAt right)
    {
        return !(left == right);
    }

    public bool Equals(CreatedAt other)
    {
        if (other == null)
            return false;

        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((CreatedAt)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}