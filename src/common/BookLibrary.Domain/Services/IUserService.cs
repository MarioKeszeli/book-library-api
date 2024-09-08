using BookLibrary.Domain.Entities;

namespace BookLibrary.Domain.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken);
}
