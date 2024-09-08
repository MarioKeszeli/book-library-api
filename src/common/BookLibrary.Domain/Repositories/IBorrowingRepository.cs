using BookLibrary.Domain.Entities;

namespace BookLibrary.Domain.Repositories;

public interface IBorrowingRepository
{
    Task CreateBorrowingAsync(Borrowing borrowing, CancellationToken cancellationToken = default);
    Task<Borrowing?> GetBorrowingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Borrowing>> GetBorrowings();
    Task<Borrowing?> UpdateBorrowingAsync(Borrowing borrowing, CancellationToken cancellationToken = default);
    Task<bool> DeleteBorrowingAsync(Guid id, CancellationToken cancellationToken = default);
}
