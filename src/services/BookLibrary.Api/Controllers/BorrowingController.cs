using BookLibrary.Domain.DTOs;
using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace BookLibrary.Api.Controllers;

/// <summary>
///   Exposes operations for managing borrowings
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BorrowingController : ControllerBase
{
    private readonly IBorrowingService _borrowingService;
    private readonly IBookService _bookService;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of <see cref="BorrowingController"/> class.
    /// </summary>
    /// <param name="borrowingService"></param>
    /// <param name="bookService"></param>
    /// <param name="userService"></param>
    /// <exception cref="ArgumentNullException">
    /// <para>The <paramref name="borrowingService"/> is <see langword="null"/>.</para>
    /// <para>- or -</para>
    /// <para>The <paramref name="bookService"/> is <see langword="null"/>.</para>
    /// <para>- or -</para>
    /// <para>The <paramref name="userService"/> is <see langword="null"/>.</para>
    /// </exception>
    public BorrowingController(IBorrowingService borrowingService, IBookService bookService, IUserService userService)
    {
        ArgumentNullException.ThrowIfNull(borrowingService);
        ArgumentNullException.ThrowIfNull(bookService);
        ArgumentNullException.ThrowIfNull(userService);

        _borrowingService = borrowingService;
        _bookService = bookService;
        _userService = userService;
    }

    /// <summary>
    /// Borrow book
    /// </summary>
    /// <param name="borrowing">Borrowing definition</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>An instance of <see cref="Borrowing"/> class.</returns>
    [HttpPost("borrow")]
    [ProducesResponseType(typeof(Borrowing), StatusCodes.Status200OK, System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<Borrowing>> Borrow(
        [FromBody][Required] BorrowingDto borrowing,
        CancellationToken cancellationToken = default)
    {
        if (borrowing.UserId == Guid.Empty)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "UserId", new[] { "The UserId field must not be empty." } }
                        }
            });
        }

        if (borrowing.BookId == Guid.Empty)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "BookId", new[] { "The BookId field must not be empty." } }
                        }
            });
        }

        if (borrowing.ReturnDate <= borrowing.BorrowDate)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "ReturnDate", new[] { "The ReturnDate value must be greater than the BorrowDate value." } }
                        }
            });
        }

        var borrowedBook = await _bookService.GetBookAsync(borrowing.BookId, cancellationToken);

        if (borrowedBook is null)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "BookId", new[] { $"The book with Id {borrowing.BookId} does not exist." } }
                        }
            });
        }

        var user = await _userService.GetUserAsync(borrowing.UserId, cancellationToken);

        if (user is null)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "UserId", new[] { $"The user with Id {borrowing.UserId} does not exist." } }
                        }
            });
        }

        if (borrowedBook.Borrowed)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "BookId", new[] { $"The book with Id {borrowing.BookId} is already borrowed." } }
                        }
            });
        }

        var newBorrowing = new Borrowing
        {
            Id = Guid.NewGuid(),
            UserId = borrowing.UserId,
            BookId = borrowing.BookId,
            BorrowDate = borrowing.BorrowDate,
            ReturnDate = borrowing.ReturnDate
        };

        var result = await _borrowingService.CreateBorrowingAsync(newBorrowing, cancellationToken);

        borrowedBook.Borrowed = true;

        await _bookService.UpdateBookAsync(borrowedBook, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Return book
    /// </summary>
    /// <param name="id">Borrowing Id</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns></returns>
    [HttpDelete("return/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> Return(
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

        var borrowing = await _borrowingService.GetBorrowingAsync(id, cancellationToken);

        if (borrowing is null)
        {
            return NotFound($"Borrowing with ID {id} does not exist.");
        }

        var borrowedBook = await _bookService.GetBookAsync(borrowing.BookId, cancellationToken);

        if (borrowedBook is not null)
        {
            borrowedBook.Borrowed = false;

            await _borrowingService.DeleteBorrowingAsync(id, cancellationToken);

            await _bookService.UpdateBookAsync(borrowedBook, cancellationToken);
        }

        return Ok();
    }
}
