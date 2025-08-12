using MediatR;
using Volcanion.Auth.Application.DTOs;
using Volcanion.Auth.Application.Features.Authentication.Commands;
using Volcanion.Auth.Application.Interfaces;

namespace Volcanion.Auth.Application.Features.Authentication.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResult?>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<TokenResult?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _tokenService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
