using Volcanion.Auth.Domain.Common;

namespace Volcanion.Auth.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    protected Role() { } // For EF Core

    public Role(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null, empty, or whitespace", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null, empty, or whitespace", nameof(description));
            
        Name = name;
        Description = description;
        IsActive = true;
    }

    public void UpdateInfo(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
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
}
