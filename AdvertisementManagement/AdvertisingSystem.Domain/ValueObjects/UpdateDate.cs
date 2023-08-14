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

    public static bool operator ==(UpdateDate date1, UpdateDate date2)
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

    public static bool operator !=(UpdateDate date1, UpdateDate date2)
    {
        return !(date1 == date2);
    }

    public static bool operator <(UpdateDate date1, UpdateDate date2)
    {
        return date1.Value < date2.Value;
    }

    public static bool operator >(UpdateDate date1, UpdateDate date2)
    {
        return date1.Value > date2.Value;
    }

    public static bool operator <=(UpdateDate date1, UpdateDate date2)
    {
        return date1.Value <= date2.Value;
    }

    public static bool operator >=(UpdateDate date1, UpdateDate date2)
    {
        return date1.Value >= date2.Value;
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