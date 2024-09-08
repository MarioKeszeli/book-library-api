using BookLibrary.Domain.DTOs;
using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace BookLibrary.Api.Controllers;

/// <summary>
///   Exposes operations for managing users
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService"></param>
    /// <exception cref="ArgumentNullException">
    /// <para>The <paramref name="userService"/> is <see langword="null"/>.</para>
    /// </exception>
    public UserController(IUserService userService)
    {
        ArgumentNullException.ThrowIfNull(userService);

        _userService = userService;
    }

    /// <summary>
    /// Create new user
    /// </summary>
    /// <param name="user">User definition</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>An instance of <see cref="User"/> class.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK, System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<User>> CreateUser(
        [FromBody][Required] UserDto user,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "Name", new[] { "The Name field must not be empty." } }
                        }
            });
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "Email", new[] { "The Email field must not be empty." } }
                        }
            });
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Name = user.Name,
            Email = user.Email
        };

        var result = await _userService.CreateUserAsync(newUser, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get user by Id
    /// </summary>
    /// <param name="id">User Id</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>An instance of <see cref="User"/> class.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK, System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<User>> GetUser(
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

        var result = await _userService.GetUserAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound($"User with ID {id} does not exist.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Update existing user
    /// </summary>
    /// <param name="user">User definition</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>An instance of <see cref="User"/> class.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK, System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<User>> UpdateUser(
        [FromBody] User user,
        CancellationToken cancellationToken = default)
    {
        if (user is null)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Errors =
                        {
                            { "User", new[] { "The user definition is empty." } }
                        }
            });
        }

        if (user.Id == Guid.Empty)
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

        var result = await _userService.UpdateUserAsync(user, cancellationToken);

        if (result is null)
        {
            return NotFound($"User with Id {user.Id} does not exist.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete user by Id
    /// </summary>
    /// <param name="id">User Id</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, System.Net.Mime.MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> DeleteUser(
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

        var deleted = await _userService.DeleteUserAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound($"User with ID {id} does not exist.");
        }

        return Ok();
    }
}
