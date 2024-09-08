using BookLibrary.Domain.DTOs;
using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace BookLibrary.Api.Controllers;

/// <summary>
///   Exposes operations for managing books
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    /// <summary>
    /// Initializes a new instance of <see cref="BookController"/> class.
    /// </summary>
    /// <param name="bookService"></param>
    /// <exception cref="ArgumentNullException">
    /// <para>The <paramref name="bookService"/> is <see langword="null"/>.</para>
    /// </exception>
    public BookController(IBookService bookService)
    {
        ArgumentNullException.ThrowIfNull(bookService);

        _bookService = bookService;
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    /// <param name="book">Book definition</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>An instance of <see cref="Book"/> class.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK, System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<Book>> CreateBook(
        [FromBody][Required] BookDto book,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(book.Title))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "Title", new[] { "The Title field must not be empty." } }
                        }
            });
        }

        if (string.IsNullOrWhiteSpace(book.Author))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "Author", new[] { "The Author field must not be empty." } }
                        }
            });
        }

        var newBook = new Book
        {
            Id = Guid.NewGuid(),
            Title = book.Title,
            Author = book.Author,
            Borrowed = false
        };

        var result = await _bookService.CreateBookAsync(newBook, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get book by Id
    /// </summary>
    /// <param name="id">Book Id</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>An instance of <see cref="Book"/> class.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK, System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<Book>> GetBook(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "Id", new[] { "The Id field must not be empty." } }
                        }
            });
        }

        var result = await _bookService.GetBookAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound($"Book with ID {id} does not exist.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Update existing book
    /// </summary>
    /// <param name="book">Book definition</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>An instance of <see cref="Book"/> class.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK, System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<Book>> UpdateBook(
        [FromBody] Book book,
        CancellationToken cancellationToken = default)
    {
        if (book is null)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "Book", new[] { "The book definition is empty." } }
                        }
            });
        }

        if (book.Id == Guid.Empty)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "Id", new[] { "The Id field must not be empty." } }
                        }
            });
        }

        var result = await _bookService.UpdateBookAsync(book, cancellationToken);

        if (result is null)
        {
            return NotFound($"Book with Id {book.Id} does not exist.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete book by Id
    /// </summary>
    /// <param name="id">Book Id</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> DeleteBook(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "Id", new[] { "The Id field must not be empty." } }
                        }
            });
        }

        var deleted = await _bookService.DeleteBookAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound($"Book with ID {id} does not exist.");
        }

        return Ok();
    }
}
