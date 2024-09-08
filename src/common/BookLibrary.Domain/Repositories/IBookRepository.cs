using BookLibrary.Domain.Entities;

namespace BookLibrary.Domain.Repositories;

public interface IBookRepository
{
    Task CreateBookAsync(Book book, CancellationToken cancellationToken = default);
    Task<Book?> GetBookAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Book?> UpdateBookAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken = default);
}
