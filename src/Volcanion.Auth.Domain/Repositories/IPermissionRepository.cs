using Volcanion.Auth.Domain.Entities;

namespace Volcanion.Auth.Domain.Repositories;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Permission permission, CancellationToken cancellationToken = default);
}
