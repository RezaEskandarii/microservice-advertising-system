namespace AdvertisingSystem.Domain.ValueObjects;

public class Address
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (Street?.GetHashCode() ?? 0);
            hash = hash * 23 + (City?.GetHashCode() ?? 0);
            hash = hash * 23 + (State?.GetHashCode() ?? 0);
            hash = hash * 23 + (PostalCode?.GetHashCode() ?? 0);
            hash = hash * 23 + (Country?.GetHashCode() ?? 0);
            return hash;
        }
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (ReferenceEquals(obj, null) || GetType() != obj.GetType())
            return false;

        var other = (Address)obj;
        return Street == other.Street &&
               City == other.City &&
               State == other.State &&
               PostalCode == other.PostalCode &&
               Country == other.Country;
    }

    public override string ToString()
    {
        return $"street: {Street}, city: {City}, state: {State}, postalCode: {PostalCode}, country: {Country}";
    }
}