using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.Book;

public class BookDelete(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _idsToDelete = [];

    [Fact]
    public async Task BookDelete_EmptyIdReturnsBadRequest()
    {
        // Arrange
        Guid id = Guid.Empty;

        // Act
        var response = await _client.DeleteAsync($"/api/book/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BookDelete_CreatedBookShouldDelete()
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

        var deleteResponse = await _client.DeleteAsync($"/api/book/{createResult!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task BookDelete_RandomIdShouldNotDelete()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/book/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
