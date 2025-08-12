using MediatR;
using AutoMapper;
using Volcanion.Auth.Application.DTOs;
using Volcanion.Auth.Application.Features.Authentication.Commands;
using Volcanion.Auth.Application.Interfaces;
using Volcanion.Auth.Domain.Repositories;
using Volcanion.Auth.Domain.Services;

namespace Volcanion.Auth.Application.Features.Authentication.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IAuthenticationService _authService;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IMapper mapper,
        IAuthenticationService authService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
        _authService = authService;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Authenticate user
            var user = await _authService.AuthenticateAsync(request.EmailOrPhone, request.Password, cancellationToken);
            
            if (user == null)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid credentials"
                };
            }

            if (!user.IsActive)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Account is deactivated"
                };
            }

            // Update last login
            user.UpdateLastLogin();
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate tokens
            var tokenResult = await _tokenService.GenerateTokenAsync(user.Id, cancellationToken);

            // Map user to DTO
            var userDto = _mapper.Map<UserDto>(user);

            return new AuthenticationResult
            {
                IsSuccess = true,
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                ExpiresAt = tokenResult.ExpiresAt,
                User = userDto
            };
        }
        catch (Exception)
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred during login"
            };
        }
    }
}
