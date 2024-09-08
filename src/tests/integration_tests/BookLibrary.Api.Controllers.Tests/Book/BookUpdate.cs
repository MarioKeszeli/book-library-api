using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.Book;

public class BookUpdate(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _idsToDelete = [];

    [Fact]
    public async Task BookUpdate_EmptyCallReturnsBadRequest()
    {
        // Arrange

        // Act
        var response = await _client.PutAsJsonAsync("/api/book", string.Empty);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BookUpdate_EmptyIdReturnsBadRequest()
    {
        // Arrange
        var book = new Domain.Entities.Book
        {
            Id = Guid.Empty
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/book/", book);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BookUpdate_RandomIdShouldNotUpdate()
    {
        // Arrange
        var book = new Domain.Entities.Book
        {
            Id = Guid.NewGuid()
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/book/", book);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task BookUpdate_CreatedBookShouldUpdate()
    {
        // Arrange
        var book = new Domain.DTOs.BookDto
        {
            Title = "Title",
            Author = "Author"
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/book/", book);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Domain.Entities.Book>();

        createResult!.Title = "TitleUpdated";
        createResult.Author = "AuthorUpdated";
        createResult.Borrowed = true;

        var updateResponse = await _client.PutAsJsonAsync("/api/book/", createResult);

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updateResult = await updateResponse.Content.ReadFromJsonAsync<Domain.Entities.Book>();

        Assert.NotNull(createResult);
        Assert.NotNull(updateResult);
        Assert.Equal(createResult.Id, updateResult.Id);
        Assert.Equal("TitleUpdated", updateResult.Title);
        Assert.Equal("AuthorUpdated", updateResult.Author);
        Assert.True(updateResult.Borrowed);

        _idsToDelete.Add(createResult.Id);
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
