using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Volcanion.Auth.Application.DTOs;
using Volcanion.Auth.Application.Interfaces;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.Repositories;

namespace Volcanion.Auth.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public TokenService(
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<TokenResult> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new ArgumentException("User not found", nameof(userId));

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes());

        // Create user session
        var userSession = new UserSession(
            userId: user.Id,
            refreshToken: refreshToken,
            deviceInfo: "Unknown", // Should be passed from request
            ipAddress: "Unknown", // Should be passed from request
            userAgent: "Unknown", // Should be passed from request
            expiresAt: DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays())
        );

        await _unitOfWork.UserSessions.AddAsync(userSession, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Store refresh token in Redis
        var cacheKey = $"refresh_token:{refreshToken}";
        var cacheValue = new { UserId = userId, SessionId = userSession.Id };
        await _cacheService.SetAsync(
            cacheKey, 
            cacheValue, 
            TimeSpan.FromDays(GetRefreshTokenExpirationDays()), 
            cancellationToken);

        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            TokenType = "Bearer"
        };
    }

    public async Task<TokenResult?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"refresh_token:{refreshToken}";
        var cachedData = await _cacheService.GetAsync<dynamic>(cacheKey, cancellationToken);
        
        if (cachedData == null)
            return null;

        var userSession = await _unitOfWork.UserSessions.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        if (userSession == null || !userSession.IsValid())
            return null;

        var user = await _unitOfWork.Users.GetByIdAsync(userSession.UserId, cancellationToken);
        if (user == null || !user.IsActive)
            return null;

        // Revoke old session
        userSession.Revoke("Token refresh");
        await _unitOfWork.UserSessions.UpdateAsync(userSession, cancellationToken);

        // Remove old refresh token from cache
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        // Generate new tokens
        return await GenerateTokenAsync(user.Id, cancellationToken);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userSession = await _unitOfWork.UserSessions.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        if (userSession == null)
            return false;

        userSession.Revoke("Manual revocation");
        await _unitOfWork.UserSessions.UpdateAsync(userSession, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Remove from cache
        var cacheKey = $"refresh_token:{refreshToken}";
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        return true;
    }

    public async Task<bool> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.UserSessions.RevokeAllUserSessionsAsync(userId, "Revoke all sessions", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Remove all user's refresh tokens from cache
        var pattern = $"refresh_token:*";
        await _cacheService.RemoveByPatternAsync(pattern, cancellationToken);

        return true;
    }

    private string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtSecret()));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email.Value),
            new(ClaimTypes.Name, user.GetFullName()),
            new("user_id", user.Id.ToString()),
        };

        // Add roles
        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            
            // Add permissions
            foreach (var rolePermission in userRole.Role.RolePermissions)
            {
                claims.Add(new Claim("permission", rolePermission.Permission.Name));
                claims.Add(new Claim("permission_action", $"{rolePermission.Permission.Resource}:{rolePermission.Permission.Action}"));
            }
        }

        var token = new JwtSecurityToken(
            issuer: GetJwtIssuer(),
            audience: GetJwtAudience(),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }

    private string GetJwtSecret()
    {
        return _configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
    }

    private string GetJwtIssuer()
    {
        return _configuration["JwtSettings:Issuer"] ?? "VolcanionAuth";
    }

    private string GetJwtAudience()
    {
        return _configuration["JwtSettings:Audience"] ?? "VolcanionAuthUsers";
    }

    private int GetAccessTokenExpirationMinutes()
    {
        return int.Parse(_configuration["JwtSettings:AccessTokenExpirationInMinutes"] ?? "15");
    }

    private int GetRefreshTokenExpirationDays()
    {
        return int.Parse(_configuration["JwtSettings:RefreshTokenExpirationInDays"] ?? "7");
    }
}
