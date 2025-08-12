using Volcanion.Auth.Domain.Common;

namespace Volcanion.Auth.Domain.ValueObjects;

public class Password : ValueObject
{
    public string HashedValue { get; private set; }

    private Password(string hashedValue)
    {
        HashedValue = hashedValue;
    }

    public static Password CreateFromHash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be null or empty", nameof(hashedPassword));

        return new Password(hashedPassword);
    }

    public static Password CreateFromPlainText(string plainTextPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
            throw new ArgumentException("Password cannot be null or empty", nameof(plainTextPassword));

        ValidatePasswordStrength(plainTextPassword);
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
        return new Password(hashedPassword);
    }

    public bool VerifyPassword(string plainTextPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
            return false;

        return BCrypt.Net.BCrypt.Verify(plainTextPassword, HashedValue);
    }

    private static void ValidatePasswordStrength(string password)
    {
        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long");

        if (!password.Any(char.IsUpper))
            throw new ArgumentException("Password must contain at least one uppercase letter");

        if (!password.Any(char.IsLower))
            throw new ArgumentException("Password must contain at least one lowercase letter");

        if (!password.Any(char.IsDigit))
            throw new ArgumentException("Password must contain at least one digit");

        if (!password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c)))
            throw new ArgumentException("Password must contain at least one special character");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HashedValue;
    }

    public override string ToString() => "[Protected]";
}
