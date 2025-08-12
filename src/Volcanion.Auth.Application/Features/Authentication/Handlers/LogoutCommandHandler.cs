using MediatR;
using Volcanion.Auth.Application.Features.Authentication.Commands;
using Volcanion.Auth.Application.Interfaces;

namespace Volcanion.Auth.Application.Features.Authentication.Handlers;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly ITokenService _tokenService;

    public LogoutCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _tokenService.RevokeTokenAsync(request.RefreshToken, cancellationToken);
        }
        catch
        {
            return false;
        }
    }
}
