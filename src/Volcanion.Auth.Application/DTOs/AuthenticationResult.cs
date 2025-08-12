namespace Volcanion.Auth.Application.DTOs;

public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }
}
