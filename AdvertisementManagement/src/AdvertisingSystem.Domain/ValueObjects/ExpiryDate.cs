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

    public static bool operator ==(ExpiryDate date1, ExpiryDate date2)
    {
        if (ReferenceEquals(date1, date2))
        {
            return true;
        }

        if (ReferenceEquals(date1, null) || ReferenceEquals(date2, null))
        {
            return false;
        }

        return date1.Value == date2.Value;
    }

    public static bool operator !=(ExpiryDate date1, ExpiryDate date2)
    {
        return !(date1 == date2);
    }

    public static bool operator <(ExpiryDate date1, ExpiryDate date2)
    {
        return date1.Value < date2.Value;
    }

    public static bool operator >(ExpiryDate date1, ExpiryDate date2)
    {
        return date1.Value > date2.Value;
    }

    public static bool operator <=(ExpiryDate date1, ExpiryDate date2)
    {
        return date1.Value <= date2.Value;
    }

    public static bool operator >=(ExpiryDate date1, ExpiryDate date2)
    {
        return date1.Value >= date2.Value;
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