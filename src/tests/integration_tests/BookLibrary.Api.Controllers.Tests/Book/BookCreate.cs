using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.Book;

public class BookCreate(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _idsToDelete = [];

    [Fact]
    public async Task BookCreate_EmptyCallReturnsBadRequest()
    {
        // Arrange

        // Act
        var response = await _client.PostAsJsonAsync("/api/book", string.Empty);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/book", null, null)]
    [InlineData("/api/book", "", "")]
    [InlineData("/api/book", "   ", "   ")]
    [InlineData("/api/book", "Title", null)]
    [InlineData("/api/book", "Title", "")]
    [InlineData("/api/book", "Title", "   ")]
    [InlineData("/api/book", null, "Author")]
    [InlineData("/api/book", "", "Author")]
    [InlineData("/api/book", "   ", "Author")]
    public async Task BookCreate_ShouldNotCreate(string url, string title, string author)
    {
        // Arrange
        var book = new Domain.DTOs.BookDto
        {
            Title = title,
            Author = author
        };

        // Act
        var response = await _client.PostAsJsonAsync(url, book);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/book", "Title", "Author")]
    public async Task BookCreate_ShouldCreate(string url, string title, string author)
    {
        // Arrange
        var book = new Domain.DTOs.BookDto
        {
            Title = title,
            Author = author
        };

        // Act
        var response = await _client.PostAsJsonAsync(url, book);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Domain.Entities.Book>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(title, result.Title);
        Assert.Equal(author, result.Author);
        Assert.False(result.Borrowed);

        _idsToDelete.Add(result.Id);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        foreach (var id in _idsToDelete)
        {
            await _client.DeleteAsync($"/api/book/{id}");
        }
    }
}
