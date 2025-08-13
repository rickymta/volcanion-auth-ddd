using AutoMapper;
using MediatR;
using Volcanion.Auth.Application.Common;
using Volcanion.Auth.Application.DTOs;
using Volcanion.Auth.Application.Features.Users.Commands;
using Volcanion.Auth.Application.Features.Users.Queries;
using Volcanion.Auth.Domain.Repositories;
using Volcanion.Auth.Domain.Services;
using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Application.Features.Users.Handlers;

public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserProfileHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdWithRolesAsync(request.UserId);
            if (user == null)
            {
                return Result<UserDto>.Failure("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Error retrieving user profile: {ex.Message}");
        }
    }
}

public class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (users, totalCount) = await _userRepository.GetPagedUsersAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.Role,
                request.IsActive,
                request.SortBy,
                request.SortDirection
            );

            var userDtos = _mapper.Map<List<UserDto>>(users);
            
            var result = new PagedResult<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return Result<PagedResult<UserDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<UserDto>>.Failure($"Error retrieving users: {ex.Message}");
        }
    }
}

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserProfileHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<UserDto>.Failure("User not found");
            }

            // Update user properties
            user.UpdateProfile(request.FirstName, request.LastName);
            
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                var phoneNumber = PhoneNumber.CreateResult(request.PhoneNumber);
                if (phoneNumber.IsFailure)
                {
                    return Result<UserDto>.Failure(phoneNumber.Error);
                }
                user.UpdatePhoneNumber(phoneNumber.Value);
            }

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Error updating user profile: {ex.Message}");
        }
    }
}

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticationService _authService;

    public ChangePasswordHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthenticationService authService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return Result<bool>.Failure("New password and confirm password do not match");
            }

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<bool>.Failure("User not found");
            }

            // Verify current password
            var isCurrentPasswordValid = user.Password.VerifyPassword(request.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                return Result<bool>.Failure("Current password is incorrect");
            }

            // Create new password
            var newPassword = Password.Create(request.NewPassword);
            if (newPassword.IsFailure)
            {
                return Result<bool>.Failure(newPassword.Error);
            }

            // Update password
            user.ChangePassword(newPassword.Value);
            
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error changing password: {ex.Message}");
        }
    }
}

public class AssignUserRolesHandler : IRequestHandler<AssignUserRolesCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignUserRolesHandler(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdWithRolesAsync(request.UserId);
            if (user == null)
            {
                return Result<bool>.Failure("User not found");
            }

            var roles = await _roleRepository.GetByIdsAsync(request.RoleIds);
            if (roles.Count != request.RoleIds.Count)
            {
                return Result<bool>.Failure("One or more roles not found");
            }

            user.AssignRoles(roles);
            
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error assigning roles: {ex.Message}");
        }
    }
}
