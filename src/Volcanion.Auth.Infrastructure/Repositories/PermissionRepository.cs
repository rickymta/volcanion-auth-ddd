using Microsoft.EntityFrameworkCore;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.Repositories;
using Volcanion.Auth.Infrastructure.Data;

namespace Volcanion.Auth.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly AuthDbContext _context;

    public PermissionRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.AnyAsync(p => p.Name == name, cancellationToken);
    }

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await _context.Permissions.AddAsync(permission, cancellationToken);
    }

    public Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Update(permission);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Remove(permission);
        return Task.CompletedTask;
    }
}
