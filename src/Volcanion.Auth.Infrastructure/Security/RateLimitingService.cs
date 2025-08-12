using System.Collections.Concurrent;

namespace Volcanion.Auth.Infrastructure.Security;

public interface IRateLimitingService
{
    Task<bool> IsRequestAllowedAsync(string identifier, int maxRequests, TimeSpan timeWindow);
    Task<bool> IsLoginAttemptAllowedAsync(string ipAddress, string? email = null);
    Task ResetLimitAsync(string identifier);
}

public class RateLimitingService : IRateLimitingService
{
    private readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();
    private readonly ConcurrentDictionary<string, int> _loginAttempts = new();

    public async Task<bool> IsRequestAllowedAsync(string identifier, int maxRequests, TimeSpan timeWindow)
    {
        await Task.CompletedTask; // Async for future Redis implementation

        var now = DateTime.UtcNow;
        var cutoff = now - timeWindow;

        _requests.AddOrUpdate(identifier, 
            new List<DateTime> { now },
            (key, existing) =>
            {
                // Remove old requests outside the time window
                existing.RemoveAll(x => x < cutoff);
                existing.Add(now);
                return existing;
            });

        return _requests[identifier].Count <= maxRequests;
    }

    public async Task<bool> IsLoginAttemptAllowedAsync(string ipAddress, string? email = null)
    {
        // Allow max 5 login attempts per IP per 15 minutes
        var ipKey = $"login_ip_{ipAddress}";
        var ipAllowed = await IsRequestAllowedAsync(ipKey, 5, TimeSpan.FromMinutes(15));

        if (!ipAllowed) return false;

        // If email provided, allow max 3 attempts per email per 15 minutes
        if (!string.IsNullOrEmpty(email))
        {
            var emailKey = $"login_email_{email}";
            var emailAllowed = await IsRequestAllowedAsync(emailKey, 3, TimeSpan.FromMinutes(15));
            return emailAllowed;
        }

        return true;
    }

    public async Task ResetLimitAsync(string identifier)
    {
        await Task.CompletedTask;
        _requests.TryRemove(identifier, out _);
    }
}
