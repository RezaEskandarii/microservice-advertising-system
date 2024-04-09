namespace AdvertisingSystem.Domain.ValueObjects;

public class Price
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Price(decimal amount, string currency)
    {
        if (amount < 0)
            throw new Exception("Amount can not be less than 0");

        Amount = amount;
        Currency = currency;
    }

    public static bool operator ==(Price left, Price right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Amount == right.Amount && left.Currency == right.Currency;
    }

    public static bool operator !=(Price left, Price right)
    {
        return !(left == right);
    }

    public static bool operator <(Price left, Price right)
    {
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Amount < right.Amount;
    }

    public static bool operator >(Price left, Price right)
    {
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Amount > right.Amount;
    }

    public static bool operator <=(Price left, Price right)
    {
        return left < right || left == right;
    }

    public static bool operator >=(Price left, Price right)
    {
        return left > right || left == right;
    }

    public static Price operator +(Price left, Price right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add prices with different currencies.");

        var sum = left.Amount + right.Amount;
        return new Price(sum, left.Currency);
    }

    public static Price operator -(Price left, Price right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract prices with different currencies.");

        var difference = left.Amount - right.Amount;
        return new Price(difference, left.Currency);
    }


    public static Price operator *(Price price, decimal multiplier)
    {
        var result = price.Amount * multiplier;
        return new Price(result, price.Currency);
    }

    public static Price operator /(Price price, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException("Cannot divide a price by zero.");

        var result = price.Amount / divisor;
        return new Price(result, price.Currency);
    }


    public override int GetHashCode()
    {
        var hash = 17;
        unchecked
        {
            hash = hash * 23 + Amount.GetHashCode();
            hash = hash * 23 + (Currency?.GetHashCode() ?? 0);
            return hash;
        }
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (ReferenceEquals(obj, null) || GetType() != obj.GetType())
            return false;

        var other = (Price)obj;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override string ToString()
    {
        return $"{Amount} {Currency}";
    }
}