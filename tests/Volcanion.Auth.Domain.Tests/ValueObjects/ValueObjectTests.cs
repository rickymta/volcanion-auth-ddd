using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Email_Create_Should_Create_Valid_Email()
    {
        // Arrange
        var emailValue = "test@example.com";

        // Act
        var email = Email.Create(emailValue);

        // Assert
        Assert.Equal(emailValue, email.Value);
        Assert.Equal(emailValue, email.ToString());
    }

    [Fact]
    public void Email_Create_Should_Throw_When_Value_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(null!));
    }

    [Fact]
    public void Email_Create_Should_Throw_When_Value_Is_Empty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(""));
    }

    [Fact]
    public void Email_Create_Should_Throw_When_Value_Is_Whitespace()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create("   "));
    }

    [Fact]
    public void Email_Create_Should_Throw_When_Invalid_Format()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create("invalid-email"));
        Assert.Throws<ArgumentException>(() => Email.Create("test@"));
        Assert.Throws<ArgumentException>(() => Email.Create("@example.com"));
        Assert.Throws<ArgumentException>(() => Email.Create("test.example.com"));
        Assert.Throws<ArgumentException>(() => Email.Create("test..test@example.com"));
        Assert.Throws<ArgumentException>(() => Email.Create(".test@example.com"));
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.com")]
    [InlineData("user123@example.org")]
    [InlineData("test@test-domain.co.uk")]
    public void Email_Create_Should_Accept_Valid_Formats(string emailValue)
    {
        // Act & Assert
        var email = Email.Create(emailValue);
        Assert.Equal(emailValue.ToLowerInvariant(), email.Value);
    }

    [Fact]
    public void Email_Equality_Should_Work_With_Same_Values()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        // Act & Assert
        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
        Assert.False(email1 != email2);
    }

    [Fact]
    public void Email_Equality_Should_Fail_With_Different_Values()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com");
        var email2 = Email.Create("test2@example.com");

        // Act & Assert
        Assert.NotEqual(email1, email2);
        Assert.False(email1 == email2);
        Assert.True(email1 != email2);
    }

    [Fact]
    public void Email_GetHashCode_Should_Be_Same_For_Equal_Values()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        // Act
        var hash1 = email1.GetHashCode();
        var hash2 = email2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Email_ImplicitOperator_Should_Convert_To_String()
    {
        // Arrange
        var emailValue = "test@example.com";
        var email = Email.Create(emailValue);

        // Act
        string result = email;

        // Assert
        Assert.Equal(emailValue, result);
    }

    [Fact]
    public void Email_Should_Normalize_To_LowerCase()
    {
        // Arrange
        var emailValue = "Test@Example.COM";

        // Act
        var email = Email.Create(emailValue);

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Email_Should_Trim_Whitespace()
    {
        // Arrange
        var emailValue = "  test@example.com  ";

        // Act
        var email = Email.Create(emailValue);

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }
}

public class PasswordTests
{
    [Fact]
    public void Password_CreateFromPlainText_Should_Create_Valid_Password()
    {
        // Arrange
        var passwordValue = "StrongPassword123!";

        // Act
        var password = Password.CreateFromPlainText(passwordValue);

        // Assert
        Assert.NotNull(password.HashedValue);
        Assert.NotEqual(passwordValue, password.HashedValue);
    }

    [Fact]
    public void Password_CreateFromHash_Should_Create_Password_From_Hash()
    {
        // Arrange
        var hashedValue = "$2a$11$abc123hashedvalue";

        // Act
        var password = Password.CreateFromHash(hashedValue);

        // Assert
        Assert.Equal(hashedValue, password.HashedValue);
    }

    [Fact]
    public void Password_CreateFromPlainText_Should_Throw_When_Value_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Password.CreateFromPlainText(null!));
    }

    [Fact]
    public void Password_CreateFromPlainText_Should_Throw_When_Value_Is_Empty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Password.CreateFromPlainText(""));
    }

    [Fact]
    public void Password_CreateFromPlainText_Should_Throw_When_Value_Is_Whitespace()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Password.CreateFromPlainText("   "));
    }

    [Fact]
    public void Password_CreateFromPlainText_Should_Throw_When_Too_Short()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Password.CreateFromPlainText("123"));
    }

    [Fact]
    public void Password_CreateFromPlainText_Should_Throw_When_Missing_Uppercase()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Password.CreateFromPlainText("password123!"));
    }

    [Fact]
    public void Password_CreateFromPlainText_Should_Throw_When_Missing_Lowercase()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Password.CreateFromPlainText("PASSWORD123!"));
    }

    [Fact]
    public void Password_CreateFromPlainText_Should_Throw_When_Missing_Number()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Password.CreateFromPlainText("Password!"));
    }

    [Fact]
    public void Password_CreateFromPlainText_Should_Throw_When_Missing_Special_Character()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Password.CreateFromPlainText("Password123"));
    }

    [Theory]
    [InlineData("StrongPassword123!")]
    [InlineData("MySecure@Password1")]
    [InlineData("Complex#Pass9")]
    [InlineData("Valid$Password2024")]
    public void Password_CreateFromPlainText_Should_Accept_Valid_Passwords(string passwordValue)
    {
        // Act & Assert
        var password = Password.CreateFromPlainText(passwordValue);
        Assert.NotNull(password.HashedValue);
    }

    [Fact]
    public void Password_VerifyPassword_Should_Return_True_For_Correct_Password()
    {
        // Arrange
        var plainTextPassword = "StrongPassword123!";
        var password = Password.CreateFromPlainText(plainTextPassword);

        // Act
        var isValid = password.VerifyPassword(plainTextPassword);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void Password_VerifyPassword_Should_Return_False_For_Incorrect_Password()
    {
        // Arrange
        var plainTextPassword = "StrongPassword123!";
        var wrongPassword = "WrongPassword456!";
        var password = Password.CreateFromPlainText(plainTextPassword);

        // Act
        var isValid = password.VerifyPassword(wrongPassword);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Password_VerifyPassword_Should_Return_False_For_Null_Password()
    {
        // Arrange
        var password = Password.CreateFromPlainText("StrongPassword123!");

        // Act
        var isValid = password.VerifyPassword(null!);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Password_VerifyPassword_Should_Return_False_For_Empty_Password()
    {
        // Arrange
        var password = Password.CreateFromPlainText("StrongPassword123!");

        // Act
        var isValid = password.VerifyPassword("");

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Password_Equality_Should_Work_With_Same_Hash()
    {
        // Arrange
        var hashedValue = "$2a$11$abc123hashedvalue";
        var password1 = Password.CreateFromHash(hashedValue);
        var password2 = Password.CreateFromHash(hashedValue);

        // Act & Assert
        Assert.Equal(password1, password2);
        Assert.True(password1 == password2);
        Assert.False(password1 != password2);
    }

    [Fact]
    public void Password_GetHashCode_Should_Be_Same_For_Equal_Values()
    {
        // Arrange
        var hashedValue = "$2a$11$abc123hashedvalue";
        var password1 = Password.CreateFromHash(hashedValue);
        var password2 = Password.CreateFromHash(hashedValue);

        // Act
        var hash1 = password1.GetHashCode();
        var hash2 = password2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }
}

public class PhoneNumberTests
{
    [Fact]
    public void PhoneNumber_Create_Should_Create_Valid_PhoneNumber()
    {
        // Arrange
        var phoneValue = "+1234567890";

        // Act
        var phone = PhoneNumber.Create(phoneValue);

        // Assert
        Assert.Equal(phoneValue, phone.Value);
        Assert.Equal(phoneValue, phone.ToString());
    }

    [Fact]
    public void PhoneNumber_Create_Should_Throw_When_Value_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create(null!));
    }

    [Fact]
    public void PhoneNumber_Create_Should_Throw_When_Value_Is_Empty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create(""));
    }

    [Fact]
    public void PhoneNumber_Create_Should_Throw_When_Value_Is_Whitespace()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create("   "));
    }

    [Fact]
    public void PhoneNumber_Create_Should_Throw_When_Invalid_Format()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create("123"));
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create("abc123"));
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create("123-456"));
    }

    [Theory]
    [InlineData("+1234567890")]
    [InlineData("+12345678901")]
    [InlineData("+123456789012")]
    [InlineData("+1234567890123")]
    [InlineData("+12345678901234")]
    public void PhoneNumber_Create_Should_Accept_Valid_Formats(string phoneValue)
    {
        // Act & Assert
        var phone = PhoneNumber.Create(phoneValue);
        Assert.Equal(phoneValue, phone.Value);
    }

    [Fact]
    public void PhoneNumber_Equality_Should_Work_With_Same_Values()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("+1234567890");
        var phone2 = PhoneNumber.Create("+1234567890");

        // Act & Assert
        Assert.Equal(phone1, phone2);
        Assert.True(phone1 == phone2);
        Assert.False(phone1 != phone2);
    }

    [Fact]
    public void PhoneNumber_Equality_Should_Fail_With_Different_Values()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("+1234567890");
        var phone2 = PhoneNumber.Create("+9876543210");

        // Act & Assert
        Assert.NotEqual(phone1, phone2);
        Assert.False(phone1 == phone2);
        Assert.True(phone1 != phone2);
    }

    [Fact]
    public void PhoneNumber_GetHashCode_Should_Be_Same_For_Equal_Values()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("+1234567890");
        var phone2 = PhoneNumber.Create("+1234567890");

        // Act
        var hash1 = phone1.GetHashCode();
        var hash2 = phone2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void PhoneNumber_Should_Trim_Whitespace()
    {
        // Arrange
        var phoneValue = "  +1234567890  ";

        // Act
        var phone = PhoneNumber.Create(phoneValue);

        // Assert
        Assert.Equal("+1234567890", phone.Value);
    }

    [Fact]
    public void PhoneNumber_ImplicitOperator_Should_Convert_To_String()
    {
        // Arrange
        var phoneValue = "+1234567890";
        var phone = PhoneNumber.Create(phoneValue);

        // Act
        string result = phone;

        // Assert
        Assert.Equal(phoneValue, result);
    }
}
