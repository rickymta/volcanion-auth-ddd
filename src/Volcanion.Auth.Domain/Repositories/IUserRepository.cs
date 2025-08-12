using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailOrPhoneAsync(string emailOrPhone, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
