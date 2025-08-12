using System.Text.RegularExpressions;
using Volcanion.Auth.Domain.Common;

namespace Volcanion.Auth.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        email = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        // More comprehensive email validation
        // Rejects consecutive dots, leading/trailing dots in local part
        const string pattern = @"^[a-zA-Z0-9](?:[a-zA-Z0-9._-]*[a-zA-Z0-9])?@[a-zA-Z0-9](?:[a-zA-Z0-9.-]*[a-zA-Z0-9])?\.[a-zA-Z]{2,}$";
        
        // Additional checks for invalid patterns
        if (email.Contains("..") || email.StartsWith('.') || email.EndsWith('.') || 
            email.Contains("@.") || email.Contains(".@"))
            return false;
            
        return Regex.IsMatch(email, pattern);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
