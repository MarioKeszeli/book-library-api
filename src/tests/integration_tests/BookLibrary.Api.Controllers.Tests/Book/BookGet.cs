using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.Book;

public class BookGet(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _idsToDelete = [];

    [Fact]
    public async Task BookGet_EmptyIdReturnsBadRequest()
    {
        // Arrange
        Guid id = Guid.Empty;

        // Act
        var response = await _client.GetAsync($"/api/book/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BookGet_CreatedBookShouldReturn()
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

        var getResponse = await _client.GetAsync($"/api/book/{createResult!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getResult = await getResponse.Content.ReadFromJsonAsync<Domain.Entities.Book>();

        Assert.NotNull(createResult);
        Assert.NotNull(getResult);
        Assert.Equal(createResult.Id, getResult.Id);
        Assert.Equal("Title", getResult.Title);
        Assert.Equal("Author", getResult.Author);
        Assert.False(getResult.Borrowed);

        _idsToDelete.Add(createResult.Id);
    }

    [Fact]
    public async Task BookGet_RandomIdShouldNotReturn()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/book/{id}");

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
