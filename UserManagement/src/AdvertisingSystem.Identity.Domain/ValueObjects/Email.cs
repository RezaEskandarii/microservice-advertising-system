namespace AdvertisingSystem.Identity.Domain.ValueObjects;

using System;
using System.Text.RegularExpressions;

public class Email
{
    public readonly string Value;

    public Email(string value)
    {
        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid email format.");

        Value = value;
    }

    private bool IsValidEmail(string email)
    {
        // Regular expression pattern for email validation
        var pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        return Regex.IsMatch(email, pattern);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (Email)obj;
        return Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(Email email1, Email email2)
    {
        return email1.Equals(email2) || email2.Value == email2.Value;
    }

    public static bool operator !=(Email email1, Email email2)
    {
        return !(email1 == email2);
    }
}