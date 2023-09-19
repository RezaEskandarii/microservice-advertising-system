namespace AdvertisingSystem.Domain.ValueObjects;

using System;

public class UpdateDate
{
    public DateTime Value { get; }

    public UpdateDate(DateTime value)
    {
        if (value > DateTime.Now)
        {
            throw new ArgumentException("Updated date cannot be in the future.", nameof(value));
        }

        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public static bool operator ==(UpdateDate left, UpdateDate right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        {
            return false;
        }

        return left.Value == right.Value;
    }

    public static bool operator !=(UpdateDate left, UpdateDate right)
    {
        return !(left == right);
    }

    public static bool operator <(UpdateDate left, UpdateDate right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >(UpdateDate left, UpdateDate right)
    {
        return left.Value > right.Value;
    }

    public static bool operator <=(UpdateDate left, UpdateDate right)
    {
        return left.Value <= right.Value;
    }

    public static bool operator >=(UpdateDate left, UpdateDate right)
    {
        return left.Value >= right.Value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is UpdateDate otherDate)
        {
            return Value.Equals(otherDate.Value);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}