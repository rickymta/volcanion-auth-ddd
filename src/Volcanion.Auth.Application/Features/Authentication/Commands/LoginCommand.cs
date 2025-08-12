using MediatR;
using Volcanion.Auth.Application.DTOs;

namespace Volcanion.Auth.Application.Features.Authentication.Commands;

public class LoginCommand : IRequest<AuthenticationResult>
{
    public string EmailOrPhone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool RememberMe { get; set; }
}
