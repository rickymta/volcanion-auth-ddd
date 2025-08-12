using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.Repositories;
using Volcanion.Auth.Domain.Services;
using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ValidateCredentialsAsync(string emailOrPhone, string password, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailOrPhoneAsync(emailOrPhone, cancellationToken);
        
        if (user == null || !user.IsActive)
            return false;

        return user.Password.VerifyPassword(password);
    }

    public async Task<User?> AuthenticateAsync(string emailOrPhone, string password, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailOrPhoneAsync(emailOrPhone, cancellationToken);
        
        if (user == null || !user.IsActive)
            return null;

        if (user.Password.VerifyPassword(password))
            return user;

        return null;
    }

    public async Task<bool> IsEmailAvailableAsync(Email email, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.Users.ExistsByEmailAsync(email, cancellationToken);
        return !exists; // Available if not exists
    }

    public async Task<bool> IsPhoneNumberAvailableAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.Users.ExistsByPhoneNumberAsync(phoneNumber, cancellationToken);
        return !exists; // Available if not exists
    }
}
