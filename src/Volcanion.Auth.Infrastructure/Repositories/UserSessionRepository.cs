using Microsoft.EntityFrameworkCore;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.Repositories;
using Volcanion.Auth.Infrastructure.Data;

namespace Volcanion.Auth.Infrastructure.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly AuthDbContext _context;

    public UserSessionRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<UserSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.UserSessions
            .Include(us => us.User)
            .FirstOrDefaultAsync(us => us.Id == id, cancellationToken);
    }

    public async Task<UserSession?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _context.UserSessions
            .Include(us => us.User)
            .FirstOrDefaultAsync(us => us.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task<IEnumerable<UserSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSessions
            .Where(us => us.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserSession>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;
        return await _context.UserSessions
            .Where(us => us.UserId == userId && 
                        !us.IsRevoked && 
                        us.ExpiresAt > currentTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        await _context.UserSessions.AddAsync(session, cancellationToken);
    }

    public Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        _context.UserSessions.Update(session);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        _context.UserSessions.Remove(session);
        return Task.CompletedTask;
    }

    public async Task DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;
        var expiredSessions = await _context.UserSessions
            .Where(us => us.ExpiresAt <= currentTime)
            .ToListAsync(cancellationToken);

        _context.UserSessions.RemoveRange(expiredSessions);
    }

    public async Task RevokeAllUserSessionsAsync(Guid userId, string? revokedBy = null, CancellationToken cancellationToken = default)
    {
        var userSessions = await _context.UserSessions
            .Where(us => us.UserId == userId && !us.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var session in userSessions)
        {
            session.Revoke(revokedBy);
        }

        _context.UserSessions.UpdateRange(userSessions);
    }
}
