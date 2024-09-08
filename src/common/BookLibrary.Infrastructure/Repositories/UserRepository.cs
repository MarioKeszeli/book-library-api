using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Repositories;

namespace BookLibrary.Infrastructure.Repositories;

public class UserRepository(CosmosDbContext cosmosDbContext) : IUserRepository
{
    private readonly CosmosDbContext _dbContext = cosmosDbContext;

    public async Task CreateUserAsync(User user,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteUserAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _dbContext
            .Users
            .FindAsync(id, cancellationToken);

        if (existingUser is null) return false;

        _dbContext.Users.Remove(existingUser);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<User?> GetUserAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext
            .Users
            .FindAsync(id, cancellationToken);

        return user;
    }

    public async Task<User?> UpdateUserAsync(User user,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _dbContext
            .Users
            .FindAsync(user.Id, cancellationToken);

        if (existingUser is null) return null;

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }
}
