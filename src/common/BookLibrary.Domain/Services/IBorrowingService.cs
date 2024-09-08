using BookLibrary.Domain.Entities;

namespace BookLibrary.Domain.Services;

public interface IBorrowingService
{
    Task<Borrowing> CreateBorrowingAsync(Borrowing borrowing, CancellationToken cancellationToken);
    Task<Borrowing?> GetBorrowingAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Borrowing>> GetBorrowings();
    Task<Borrowing?> UpdateBorrowingAsync(Borrowing borrowing, CancellationToken cancellationToken);
    Task<bool> DeleteBorrowingAsync(Guid id, CancellationToken cancellationToken);
}
