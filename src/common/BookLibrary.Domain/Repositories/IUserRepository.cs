using BookLibrary.Domain.Entities;

namespace BookLibrary.Domain.Repositories;

public interface IUserRepository
{
    Task CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}
