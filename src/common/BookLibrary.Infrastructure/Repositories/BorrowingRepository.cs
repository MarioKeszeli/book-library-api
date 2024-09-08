using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Repositories;

namespace BookLibrary.Infrastructure.Repositories;

public class BorrowingRepository(CosmosDbContext cosmosDbContext) : IBorrowingRepository
{
    private readonly CosmosDbContext _dbContext = cosmosDbContext;

    public async Task CreateBorrowingAsync(Borrowing borrowing, CancellationToken cancellationToken = default)
    {
        _dbContext.Borrowings.Add(borrowing);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteBorrowingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingBorrowing = await _dbContext
            .Borrowings
            .FindAsync(id, cancellationToken);

        if (existingBorrowing is null) return false;

        _dbContext.Borrowings.Remove(existingBorrowing);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<Borrowing?> GetBorrowingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var borrowing = await _dbContext
            .Borrowings
            .FindAsync(id, cancellationToken);

        return borrowing;
    }

    public Task<List<Borrowing>> GetBorrowings()
    {
        return Task.FromResult(_dbContext.Borrowings.ToList());
    }

    public async Task<Borrowing?> UpdateBorrowingAsync(Borrowing borrowing, CancellationToken cancellationToken = default)
    {
        var existingBorrowing = await _dbContext
            .Borrowings
            .FindAsync(borrowing.Id, cancellationToken);

        if (existingBorrowing is null) return null;

        existingBorrowing.ReturnDate = borrowing.ReturnDate;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return borrowing;
    }
}
