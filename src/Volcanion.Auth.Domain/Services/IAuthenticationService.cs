using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Domain.Services;

public interface IAuthenticationService
{
    Task<bool> ValidateCredentialsAsync(string emailOrPhone, string password, CancellationToken cancellationToken = default);
    Task<User?> AuthenticateAsync(string emailOrPhone, string password, CancellationToken cancellationToken = default);
    Task<bool> IsEmailAvailableAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> IsPhoneNumberAvailableAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
}
