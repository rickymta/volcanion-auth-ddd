using Microsoft.EntityFrameworkCore;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.Repositories;
using Volcanion.Auth.Domain.ValueObjects;
using Volcanion.Auth.Infrastructure.Data;

namespace Volcanion.Auth.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task<User?> GetByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.PhoneNumber!.Value == phoneNumber.Value, cancellationToken);
    }

    public async Task<User?> GetByEmailOrPhoneAsync(string emailOrPhone, CancellationToken cancellationToken = default)
    {
        // First try to find by email
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email.Value == emailOrPhone, cancellationToken);

        // If not found by email, try by phone number using raw SQL or split the query
        if (user == null)
        {
            var usersWithPhone = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .ToListAsync(cancellationToken);
                
            user = usersWithPhone.FirstOrDefault(u => u.PhoneNumber?.Value == emailOrPhone);
        }

        return user;
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.PhoneNumber!.Value == phoneNumber.Value, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.CountAsync(cancellationToken);
    }
}
