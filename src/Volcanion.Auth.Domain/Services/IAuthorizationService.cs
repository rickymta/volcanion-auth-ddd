using Volcanion.Auth.Domain.Entities;

namespace Volcanion.Auth.Domain.Services;

public interface IAuthorizationService
{
    Task<bool> HasPermissionAsync(Guid userId, string resource, string action, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId, string? assignedBy = null, CancellationToken cancellationToken = default);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, string? assignedBy = null, CancellationToken cancellationToken = default);
    Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
}
