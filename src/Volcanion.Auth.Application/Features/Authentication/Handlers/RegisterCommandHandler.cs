using MediatR;
using AutoMapper;
using Volcanion.Auth.Application.DTOs;
using Volcanion.Auth.Application.Features.Authentication.Commands;
using Volcanion.Auth.Application.Interfaces;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.Repositories;
using Volcanion.Auth.Domain.ValueObjects;
using Volcanion.Auth.Domain.Services;

namespace Volcanion.Auth.Application.Features.Authentication.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IAuthenticationService _authService;

    public RegisterCommandHandler(
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

    public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate email uniqueness
            var email = Email.Create(request.Email);
            var emailExists = await _authService.IsEmailAvailableAsync(email, cancellationToken);
            if (!emailExists)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Email already exists"
                };
            }

            // Validate phone number uniqueness if provided
            PhoneNumber? phoneNumber = null;
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                phoneNumber = PhoneNumber.Create(request.PhoneNumber);
                var phoneExists = await _authService.IsPhoneNumberAvailableAsync(phoneNumber, cancellationToken);
                if (!phoneExists)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Phone number already exists"
                    };
                }
            }

            // Create password
            var password = Password.CreateFromPlainText(request.Password);

            // Create user
            var user = new User(request.FirstName, request.LastName, email, password);
            if (phoneNumber is not null)
            {
                user.UpdatePersonalInfo(request.FirstName, request.LastName, phoneNumber);
            }

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
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
        catch (ArgumentException ex)
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
        catch (Exception)
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred during registration"
            };
        }
    }
}
