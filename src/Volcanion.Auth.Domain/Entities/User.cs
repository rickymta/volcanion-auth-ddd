using Volcanion.Auth.Domain.Common;
using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public PhoneNumber? PhoneNumber { get; private set; }
    public Password Password { get; private set; } = null!;
    public string? AvatarUrl { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<UserSession> UserSessions { get; private set; } = new List<UserSession>();

    protected User() { } // For EF Core

    public User(string firstName, string lastName, Email email, Password password)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        IsActive = true;
        IsEmailVerified = false;
        IsPhoneVerified = false;
    }

    public static User Create(Email email, Password password, string firstName, string lastName)
    {
        return new User(firstName, lastName, email, password);
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        MarkAsUpdated();
    }

    public void UpdatePhoneNumber(PhoneNumber phoneNumber)
    {
        PhoneNumber = phoneNumber;
        MarkAsUpdated();
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword ?? throw new ArgumentNullException(nameof(newPassword));
        MarkAsUpdated();
    }

    public void AssignRoles(IEnumerable<Role> roles)
    {
        UserRoles.Clear();
        foreach (var role in roles)
        {
            UserRoles.Add(new UserRole(Id, role.Id));
        }
        MarkAsUpdated();
    }

    public void UpdatePersonalInfo(string firstName, string lastName, PhoneNumber? phoneNumber = null)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        PhoneNumber = phoneNumber;
        MarkAsUpdated();
    }

    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
        IsEmailVerified = false; // Reset verification status
        MarkAsUpdated();
    }

    public void UpdatePassword(Password newPassword)
    {
        Password = newPassword ?? throw new ArgumentNullException(nameof(newPassword));
        MarkAsUpdated();
    }

    public void UpdateAvatar(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
        MarkAsUpdated();
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        MarkAsUpdated();
    }

    public void VerifyPhone()
    {
        IsPhoneVerified = true;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public string GetFullName() => $"{FirstName} {LastName}".Trim();
}
