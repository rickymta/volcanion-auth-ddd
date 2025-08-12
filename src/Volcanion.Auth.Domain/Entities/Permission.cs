using Volcanion.Auth.Domain.Common;

namespace Volcanion.Auth.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Resource { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    protected Permission() { } // For EF Core

    public Permission(string name, string description, string resource, string action)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null, empty, or whitespace", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null, empty, or whitespace", nameof(description));
        if (string.IsNullOrWhiteSpace(resource))
            throw new ArgumentException("Resource cannot be null, empty, or whitespace", nameof(resource));
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be null, empty, or whitespace", nameof(action));
            
        Name = name;
        Description = description;
        Resource = resource;
        Action = action;
        IsActive = true;
    }

    public void UpdateInfo(string name, string description, string resource, string action)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        Action = action ?? throw new ArgumentNullException(nameof(action));
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
