using BookLibrary.Domain.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.Borrowing;

public class BorrowingBorrow(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _borrowingIdsToDelete = [];
    private readonly List<Guid> _userIdsToDelete = [];
    private readonly List<Guid> _bookIdsToDelete = [];

    [Theory]
    [MemberData(nameof(BorrowingBorrow_BadRequest_Data))]
    public async Task BorrowingBorrow_ShouldNotBorrow(Guid userId, Guid bookId, DateTimeOffset borrowDate, DateTimeOffset returnDate)
    {
        // Arrange
        var borrowing = new Domain.DTOs.BorrowingDto
        {
            UserId = userId,
            BookId = bookId,
            BorrowDate = borrowDate,
            ReturnDate = returnDate
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/borrowing/borrow", borrowing);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BorrowingBorrow_ShouldBorrow()
    {
        // Arrange
        var borrowDate = DateTimeOffset.UtcNow;
        var returnDate = DateTimeOffset.UtcNow.AddDays(30);

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
            BorrowDate = borrowDate,
            ReturnDate = returnDate
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/borrowing/borrow", borrowing);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Domain.Entities.Borrowing>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(createUserResult.Id, result.UserId);
        Assert.Equal(createBookResult.Id, result.BookId);
        Assert.Equal(borrowDate, result.BorrowDate);
        Assert.Equal(returnDate, result.ReturnDate);

        _borrowingIdsToDelete.Add(result.Id);
        _userIdsToDelete.Add(createUserResult.Id);
        _bookIdsToDelete.Add(createBookResult.Id);
    }

    public static IEnumerable<object?[]> BorrowingBorrow_BadRequest_Data()
    {
        var datetime = DateTimeOffset.UtcNow;

        return new List<object?[]>
        {
            new object?[] { Guid.Empty, Guid.Empty, datetime, datetime },
            new object?[] { Guid.Empty, Guid.Empty, datetime.AddDays(1), datetime },

            new object?[] { Guid.Empty, Guid.NewGuid(), datetime, datetime },
            new object?[] { Guid.Empty, Guid.NewGuid(), datetime.AddDays(1), datetime },

            new object?[] { Guid.NewGuid(), Guid.Empty, datetime, datetime },
            new object?[] { Guid.NewGuid(), Guid.Empty, datetime.AddDays(1), datetime },

            new object?[] { Guid.NewGuid(), Guid.NewGuid(), datetime, datetime },
            new object?[] { Guid.NewGuid(), Guid.NewGuid(), datetime.AddDays(1), datetime },
        };
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        foreach (var id in _borrowingIdsToDelete)
        {
            await _client.DeleteAsync($"/api/borrowing/return/{id}");
        }

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
