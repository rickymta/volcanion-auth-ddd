using MediatR;

namespace Volcanion.Auth.Application.Features.Authentication.Commands;

public class LogoutCommand : IRequest<bool>
{
    public string RefreshToken { get; set; } = string.Empty;
    public string? UserId { get; set; }
}
