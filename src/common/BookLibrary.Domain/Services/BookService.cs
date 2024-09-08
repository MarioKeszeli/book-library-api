using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Repositories;

namespace BookLibrary.Domain.Services;

public class BookService(IBookRepository bookRepository) : IBookService
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<Book> CreateBookAsync(Book book, CancellationToken cancellationToken)
    {
        await _bookRepository.CreateBookAsync(book, cancellationToken);

        return book;
    }

    public async Task<Book?> GetBookAsync(Guid id, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetBookAsync(id, cancellationToken);

        return book;
    }

    public async Task<Book?> UpdateBookAsync(Book book, CancellationToken cancellationToken)
    {
        var result = await _bookRepository.UpdateBookAsync(book, cancellationToken);

        return result;
    }

    public async Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bookRepository.DeleteBookAsync(id, cancellationToken);

        return result;
    }
}
