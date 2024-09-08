using BookLibrary.Domain.Entities;

namespace BookLibrary.Domain.Services;

public interface IBookService
{
    Task<Book> CreateBookAsync(Book book, CancellationToken cancellationToken);
    Task<Book?> GetBookAsync(Guid id, CancellationToken cancellationToken);
    Task<Book?> UpdateBookAsync(Book book, CancellationToken cancellationToken);
    Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken);
}
