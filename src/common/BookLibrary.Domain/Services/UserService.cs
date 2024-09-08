using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Repositories;

namespace BookLibrary.Domain.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        await _userRepository.CreateUserAsync(user, cancellationToken);

        return user;
    }

    public async Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(id, cancellationToken);

        return user;
    }

    public async Task<User?> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        var result = await _userRepository.UpdateUserAsync(user, cancellationToken);

        return result;
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userRepository.DeleteUserAsync(id, cancellationToken);

        return result;
    }
}
