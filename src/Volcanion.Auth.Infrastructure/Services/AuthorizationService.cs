using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.Repositories;
using Volcanion.Auth.Domain.Services;

namespace Volcanion.Auth.Infrastructure.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthorizationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string resource, string action, CancellationToken cancellationToken = default)
    {
        var permissions = await _unitOfWork.Permissions.GetByUserIdAsync(userId, cancellationToken);
        
        return permissions.Any(p => p.Resource == resource && 
                                   p.Action == action && 
                                   p.IsActive);
    }

    public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Permissions.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Roles.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, string? assignedBy = null, CancellationToken cancellationToken = default)
    {
        var userRole = new UserRole(userId, roleId, assignedBy);
        // Note: Need to add UserRole repository or add through context directly
        // For now, this is a placeholder implementation
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        // This would need a UserRole repository method to find and remove specific UserRole
        // For now, we'll implement a basic version
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole != null)
            {
                userRole.MarkAsDeleted();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, string? assignedBy = null, CancellationToken cancellationToken = default)
    {
        var rolePermission = new RolePermission(roleId, permissionId, assignedBy);
        // Note: Need to add RolePermission repository or add through context directly
        // For now, this is a placeholder implementation
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(roleId, cancellationToken);
        if (role != null)
        {
            var rolePermission = role.RolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
            if (rolePermission != null)
            {
                rolePermission.MarkAsDeleted();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
