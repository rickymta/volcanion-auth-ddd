using Volcanion.Auth.Domain.Common;

namespace Volcanion.Auth.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public string? AssignedBy { get; private set; }
    
    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual Role Role { get; private set; } = null!;

    protected UserRole() { } // For EF Core

    public UserRole(Guid userId, Guid roleId, string? assignedBy = null)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }
}
