using Volcanion.Auth.Domain.Common;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.Events;
using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Domain.Aggregates;

/// <summary>
/// User Aggregate Root - encapsulates user-related business logic
/// </summary>
public class UserAggregate : BaseEntity
{
    private readonly List<DomainEvent> _domainEvents = new();
    
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

    private readonly List<UserRole> _userRoles = new();
    private readonly List<UserSession> _userSessions = new();

    public IReadOnlyList<UserRole> UserRoles => _userRoles.AsReadOnly();
    public IReadOnlyList<UserSession> UserSessions => _userSessions.AsReadOnly();
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private UserAggregate() { } // EF Constructor

    public static UserAggregate Create(string firstName, string lastName, Email email, 
        Password password, PhoneNumber? phoneNumber = null)
    {
        var user = new UserAggregate
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password,
            PhoneNumber = phoneNumber,
            IsEmailVerified = false,
            IsPhoneVerified = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, email.Value, firstName, lastName));
        return user;
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new PasswordChangedEvent(Id, DateTime.UtcNow));
    }

    public void UpdateProfile(string firstName, string lastName, PhoneNumber? phoneNumber = null)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyPhone()
    {
        IsPhoneVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin(string ipAddress, string userAgent)
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserLoggedInEvent(Id, ipAddress, userAgent, DateTime.UtcNow));
    }

    public void AssignRole(Role role, Guid assignedBy)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.Id))
            return; // Already has this role

        // Note: In a real implementation, this would be handled by the application service
        // The aggregate focuses on business rules, not entity creation
        AddDomainEvent(new RoleAssignedEvent(Id, role.Id, role.Name, assignedBy));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
