using MediatR;
using Volcanion.Auth.Application.DTOs;

namespace Volcanion.Auth.Application.Features.Authentication.Commands;

public class RegisterCommand : IRequest<AuthenticationResult>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
