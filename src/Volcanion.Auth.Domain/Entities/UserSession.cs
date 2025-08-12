using Volcanion.Auth.Domain.Common;

namespace Volcanion.Auth.Domain.Entities;

public class UserSession : BaseEntity
{
    public Guid UserId { get; private set; }
    public string RefreshToken { get; private set; } = string.Empty;
    public string DeviceInfo { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public DateTime LastAccessedAt { get; private set; }
    
    // Navigation properties
    public virtual User User { get; private set; } = null!;

    protected UserSession() { } // For EF Core

    public UserSession(Guid userId, string refreshToken, string deviceInfo, string ipAddress, 
        string userAgent, DateTime expiresAt)
    {
        UserId = userId;
        RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
        DeviceInfo = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
        IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
        ExpiresAt = expiresAt;
        IsRevoked = false;
        LastAccessedAt = DateTime.UtcNow;
    }

    public void UpdateLastAccess()
    {
        LastAccessedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void Revoke(string? revokedBy = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedBy = revokedBy;
        MarkAsUpdated();
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    public bool IsValid() => !IsRevoked && !IsExpired();
}
