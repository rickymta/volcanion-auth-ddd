using System.Text.RegularExpressions;
using Volcanion.Auth.Domain.Common;

namespace Volcanion.Auth.Domain.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; private set; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be null or empty", nameof(phoneNumber));

        phoneNumber = phoneNumber.Trim();

        if (!IsValidPhoneNumber(phoneNumber))
            throw new ArgumentException("Invalid phone number format", nameof(phoneNumber));

        return new PhoneNumber(phoneNumber);
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Support international phone number formats
        // Pattern: +[country code][number] where total length is 7-15 digits after +
        // Examples: +1234567890, +447911123456, +84901234567
        const string pattern = @"^\+[1-9]\d{6,14}$";
        return Regex.IsMatch(phoneNumber, pattern);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
