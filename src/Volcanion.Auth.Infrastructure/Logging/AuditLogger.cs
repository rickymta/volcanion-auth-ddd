using Microsoft.Extensions.Logging;

namespace Volcanion.Auth.Infrastructure.Logging;

public interface IAuditLogger
{
    Task LogCreateAsync<T>(string userId, T entity, string entityId);
    Task LogUpdateAsync<T>(string userId, T oldEntity, T newEntity, string entityId);
    Task LogDeleteAsync(string userId, string entityType, string entityId);
    Task LogLoginAsync(string userId, string ipAddress, string userAgent);
    Task LogLogoutAsync(string userId, string sessionId);
}

public class AuditLogger : IAuditLogger
{
    private readonly IElasticsearchLogger _elasticsearchLogger;
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(IElasticsearchLogger elasticsearchLogger, ILogger<AuditLogger> logger)
    {
        _elasticsearchLogger = elasticsearchLogger;
        _logger = logger;
    }

    public async Task LogCreateAsync<T>(string userId, T entity, string entityId)
    {
        var auditData = new
        {
            Action = "CREATE",
            EntityType = typeof(T).Name,
            EntityId = entityId,
            UserId = userId,
            NewValue = entity,
            Timestamp = DateTime.UtcNow
        };

        await _elasticsearchLogger.LogAsync(LogLevel.Information, 
            $"Entity created: {typeof(T).Name}", auditData);
    }

    public async Task LogUpdateAsync<T>(string userId, T oldEntity, T newEntity, string entityId)
    {
        var auditData = new
        {
            Action = "UPDATE",
            EntityType = typeof(T).Name,
            EntityId = entityId,
            UserId = userId,
            OldValue = oldEntity,
            NewValue = newEntity,
            Timestamp = DateTime.UtcNow
        };

        await _elasticsearchLogger.LogAsync(LogLevel.Information, 
            $"Entity updated: {typeof(T).Name}", auditData);
    }

    public async Task LogDeleteAsync(string userId, string entityType, string entityId)
    {
        var auditData = new
        {
            Action = "DELETE",
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };

        await _elasticsearchLogger.LogAsync(LogLevel.Information, 
            $"Entity deleted: {entityType}", auditData);
    }

    public async Task LogLoginAsync(string userId, string ipAddress, string userAgent)
    {
        var loginData = new
        {
            Action = "LOGIN",
            UserId = userId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.UtcNow
        };

        await _elasticsearchLogger.LogSecurityEventAsync("USER_LOGIN", 
            "User logged in", loginData);
    }

    public async Task LogLogoutAsync(string userId, string sessionId)
    {
        var logoutData = new
        {
            Action = "LOGOUT",
            UserId = userId,
            SessionId = sessionId,
            Timestamp = DateTime.UtcNow
        };

        await _elasticsearchLogger.LogSecurityEventAsync("USER_LOGOUT", 
            "User logged out", logoutData);
    }
}
