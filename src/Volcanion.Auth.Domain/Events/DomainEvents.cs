namespace Volcanion.Auth.Domain.Events;

public abstract class DomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public class UserRegisteredEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public UserRegisteredEvent(Guid userId, string email, string firstName, string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}

public class UserLoggedInEvent : DomainEvent
{
    public Guid UserId { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public DateTime LoginTime { get; }

    public UserLoggedInEvent(Guid userId, string ipAddress, string userAgent, DateTime loginTime)
    {
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        LoginTime = loginTime;
    }
}

public class UserLoggedOutEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid SessionId { get; }
    public DateTime LogoutTime { get; }

    public UserLoggedOutEvent(Guid userId, Guid sessionId, DateTime logoutTime)
    {
        UserId = userId;
        SessionId = sessionId;
        LogoutTime = logoutTime;
    }
}

public class PasswordChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public DateTime ChangedAt { get; }

    public PasswordChangedEvent(Guid userId, DateTime changedAt)
    {
        UserId = userId;
        ChangedAt = changedAt;
    }
}

public class RoleAssignedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid RoleId { get; }
    public string RoleName { get; }
    public Guid AssignedBy { get; }

    public RoleAssignedEvent(Guid userId, Guid roleId, string roleName, Guid assignedBy)
    {
        UserId = userId;
        RoleId = roleId;
        RoleName = roleName;
        AssignedBy = assignedBy;
    }
}
