using BookLibrary.Domain.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.Borrowing;

public class BorrowingReturn(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _userIdsToDelete = [];
    private readonly List<Guid> _bookIdsToDelete = [];

    [Fact]
    public async Task BorrowingReturn_EmptyIdReturnsBadRequest()
    {
        // Arrange
        Guid id = Guid.Empty;

        // Act
        var response = await _client.DeleteAsync($"/api/borrowing/return/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BorrowingReturn_CreatedBorrowingShouldDelete()
    {
        // Arrange
        var book = new BookDto
        {
            Author = "Author",
            Title = "Title"
        };

        var createBookResponse = await _client.PostAsJsonAsync("/api/book", book);
        var createBookResult = await createBookResponse.Content.ReadFromJsonAsync<Domain.Entities.Book>();

        var user = new Domain.DTOs.UserDto
        {
            Name = "Name",
            Email = "email@domain.com"
        };

        var createUserResponse = await _client.PostAsJsonAsync("/api/user", user);
        var createUserResult = await createUserResponse.Content.ReadFromJsonAsync<Domain.Entities.User>();

        var borrowing = new Domain.DTOs.BorrowingDto
        {
            UserId = createUserResult!.Id,
            BookId = createBookResult!.Id,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.Now.AddDays(30)
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/borrowing/borrow/", borrowing);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Domain.Entities.Borrowing>();

        var deleteResponse = await _client.DeleteAsync($"/api/borrowing/return/{createResult!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task BorrowingReturn_RandomIdShouldNotDelete()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/borrowing/return/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        foreach (var id in _userIdsToDelete)
        {
            await _client.DeleteAsync($"/api/user/{id}");
        }

        foreach (var id in _bookIdsToDelete)
        {
            await _client.DeleteAsync($"/api/book/{id}");
        }
    }
}
