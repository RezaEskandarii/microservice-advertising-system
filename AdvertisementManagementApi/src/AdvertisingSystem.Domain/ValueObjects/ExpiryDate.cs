namespace AdvertisingSystem.Domain.ValueObjects;

using System;

public class ExpiryDate
{
    public DateTime Value { get; }

    public ExpiryDate(DateTime value)
    {
        if (value < DateTime.Now.AddDays(-1))
        {
          //  throw new ArgumentException("Expiry date cannot be in the past.", nameof(value));
        }

        Value = value.Date;
    }

    public bool IsExpired()
    {
        return Value < DateTime.Today;
    }

    public override string ToString()
    {
        return Value.ToString("yyyy-MM-dd");
    }

    public static bool operator ==(ExpiryDate left, ExpiryDate right)
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

    public static bool operator !=(ExpiryDate left, ExpiryDate right)
    {
        return !(left == right);
    }

    public static bool operator <(ExpiryDate left, ExpiryDate right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >(ExpiryDate left, ExpiryDate right)
    {
        return left.Value > right.Value;
    }

    public static bool operator <=(ExpiryDate left, ExpiryDate right)
    {
        return left.Value <= right.Value;
    }

    public static bool operator >=(ExpiryDate left, ExpiryDate right)
    {
        return left.Value >= right.Value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is ExpiryDate otherDate)
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