using Volcanion.Auth.Domain.Entities;

namespace Volcanion.Auth.Domain.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Role>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<(List<Role> roles, int totalCount)> GetPagedRolesAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null,
        string? sortBy = null,
        string? sortDirection = null,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    void Add(Role role);
    Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
    void Update(Role role);
    Task DeleteAsync(Role role, CancellationToken cancellationToken = default);
}
