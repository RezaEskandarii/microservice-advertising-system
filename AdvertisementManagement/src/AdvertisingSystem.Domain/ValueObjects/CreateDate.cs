namespace AdvertisingSystem.Domain.ValueObjects;

using System;

public class CreateDate
{
    public DateTime Value { get; }

    public CreateDate(DateTime value)
    {
        if (value > DateTime.Now)
        {
            throw new ArgumentException("Create date cannot be in the future.", nameof(value));
        }

        Value = value.Date;
    }

    public override string ToString()
    {
        return Value.ToString("yyyy-MM-dd");
    }

    public static bool operator ==(CreateDate left, CreateDate right)
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

    public static bool operator !=(CreateDate left, CreateDate right)
    {
        return !(left == right);
    }

    public static bool operator <(CreateDate left, CreateDate right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >(CreateDate left, CreateDate right)
    {
        return left.Value > right.Value;
    }

    public static bool operator <=(CreateDate left, CreateDate right)
    {
        return left.Value <= right.Value;
    }

    public static bool operator >=(CreateDate left, CreateDate right)
    {
        return left.Value >= right.Value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is CreateDate otherDate)
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