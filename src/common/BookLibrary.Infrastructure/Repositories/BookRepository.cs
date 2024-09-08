using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Repositories;

namespace BookLibrary.Infrastructure.Repositories;

public class BookRepository(CosmosDbContext cosmosDbContext) : IBookRepository
{
    private readonly CosmosDbContext _dbContext = cosmosDbContext;

    public async Task CreateBookAsync(Book book,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Books.Add(book);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteBookAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var existingBook = await _dbContext
            .Books
            .FindAsync(id, cancellationToken);

        if (existingBook is null) return false;

        _dbContext.Books.Remove(existingBook);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<Book?> GetBookAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var book = await _dbContext
            .Books
            .FindAsync(id, cancellationToken);

        return book;
    }

    public async Task<Book?> UpdateBookAsync(Book book,
        CancellationToken cancellationToken = default)
    {
        var existingBook = await _dbContext
            .Books
            .FindAsync(book.Id, cancellationToken);

        if (existingBook is null) return null;

        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Borrowed = book.Borrowed;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return book;
    }
}
