using MediatR;
using Volcanion.Auth.Application.DTOs;

namespace Volcanion.Auth.Application.Features.Authentication.Commands;

public class RefreshTokenCommand : IRequest<TokenResult?>
{
    public string RefreshToken { get; set; } = string.Empty;
}
