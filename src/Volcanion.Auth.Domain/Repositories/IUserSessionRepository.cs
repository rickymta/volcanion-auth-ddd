using Volcanion.Auth.Domain.Entities;

namespace Volcanion.Auth.Domain.Repositories;

public interface IUserSessionRepository
{
    Task<UserSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSession>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserSession session, CancellationToken cancellationToken = default);
    Task DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default);
    Task RevokeAllUserSessionsAsync(Guid userId, string? revokedBy = null, CancellationToken cancellationToken = default);
}
