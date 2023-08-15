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

    public static bool operator ==(Price price1, Price price2)
    {
        if (ReferenceEquals(price1, price2))
            return true;

        if (ReferenceEquals(price1, null) || ReferenceEquals(price2, null))
            return false;

        return price1.Amount == price2.Amount && price1.Currency == price2.Currency;
    }

    public static bool operator !=(Price price1, Price price2)
    {
        return !(price1 == price2);
    }

    public static bool operator <(Price price1, Price price2)
    {
        if (ReferenceEquals(price1, null) || ReferenceEquals(price2, null))
            return false;

        return price1.Amount < price2.Amount;
    }

    public static bool operator >(Price price1, Price price2)
    {
        if (ReferenceEquals(price1, null) || ReferenceEquals(price2, null))
            return false;

        return price1.Amount > price2.Amount;
    }

    public static bool operator <=(Price price1, Price price2)
    {
        return price1 < price2 || price1 == price2;
    }

    public static bool operator >=(Price price1, Price price2)
    {
        return price1 > price2 || price1 == price2;
    }

    public static Price operator +(Price price1, Price price2)
    {
        if (price1.Currency != price2.Currency)
            throw new InvalidOperationException("Cannot add prices with different currencies.");

        var sum = price1.Amount + price2.Amount;
        return new Price(sum, price1.Currency);
    }

    public static Price operator -(Price price1, Price price2)
    {
        if (price1.Currency != price2.Currency)
            throw new InvalidOperationException("Cannot subtract prices with different currencies.");

        var difference = price1.Amount - price2.Amount;
        return new Price(difference, price1.Currency);
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