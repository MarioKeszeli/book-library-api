using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Repositories;

namespace BookLibrary.Domain.Services;

public class BorrowingService(IBorrowingRepository borrowingRepository) : IBorrowingService
{
    private readonly IBorrowingRepository _borrowingRepository = borrowingRepository;

    public async Task<Borrowing> CreateBorrowingAsync(Borrowing borrowing, CancellationToken cancellationToken)
    {
        await _borrowingRepository.CreateBorrowingAsync(borrowing, cancellationToken);

        return borrowing;
    }

    public async Task<Borrowing?> GetBorrowingAsync(Guid id, CancellationToken cancellationToken)
    {
        var borrowing = await _borrowingRepository.GetBorrowingAsync(id, cancellationToken);

        return borrowing;
    }

    public Task<List<Borrowing>> GetBorrowings()
    {
        var borrowings = _borrowingRepository.GetBorrowings();

        return borrowings;
    }

    public async Task<Borrowing?> UpdateBorrowingAsync(Borrowing borrowing, CancellationToken cancellationToken)
    {
        var result = await _borrowingRepository.UpdateBorrowingAsync(borrowing, cancellationToken);

        return result;
    }

    public async Task<bool> DeleteBorrowingAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _borrowingRepository.DeleteBorrowingAsync(id, cancellationToken);

        return result;
    }
}
