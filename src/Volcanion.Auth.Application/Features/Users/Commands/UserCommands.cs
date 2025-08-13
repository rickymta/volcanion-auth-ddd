using MediatR;
using Volcanion.Auth.Application.Common;
using Volcanion.Auth.Application.DTOs;

namespace Volcanion.Auth.Application.Features.Users.Commands;

public class UpdateUserProfileCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

public class ChangePasswordCommand : IRequest<Result<bool>>
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class AssignUserRolesCommand : IRequest<Result<bool>>
{
    public Guid UserId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}
