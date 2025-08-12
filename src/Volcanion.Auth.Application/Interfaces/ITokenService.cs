using Volcanion.Auth.Application.DTOs;

namespace Volcanion.Auth.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResult> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TokenResult?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
