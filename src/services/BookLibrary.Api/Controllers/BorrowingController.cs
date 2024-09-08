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

    /// <summary>
    /// Initializes a new instance of <see cref="BorrowingController"/> class.
    /// </summary>
    /// <param name="borrowingService"></param>
    /// <exception cref="ArgumentNullException">
    /// <para>The <paramref name="borrowingService"/> is <see langword="null"/>.</para>
    /// </exception>
    public BorrowingController(IBorrowingService borrowingService)
    {
        ArgumentNullException.ThrowIfNull(borrowingService);

        _borrowingService = borrowingService;
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

        var newBorrowing = new Borrowing
        {
            Id = Guid.NewGuid(),
            UserId = borrowing.UserId,
            BookId = borrowing.BookId,
            BorrowDate = borrowing.BorrowDate,
            ReturnDate = borrowing.ReturnDate
        };

        var result = await _borrowingService.CreateBorrowingAsync(newBorrowing, cancellationToken);

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

        var deleted = await _borrowingService.DeleteBorrowingAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound($"Borrowing with ID {id} does not exist.");
        }

        return Ok();
    }
}
