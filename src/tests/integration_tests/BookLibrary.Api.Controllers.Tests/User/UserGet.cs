using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.User;

public class UserGet(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _idsToDelete = [];

    [Fact]
    public async Task UserGet_EmptyIdReturnsBadRequest()
    {
        // Arrange
        Guid id = Guid.Empty;

        // Act
        var response = await _client.GetAsync($"/api/user/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UserGet_CreatedUserShouldReturn()
    {
        // Arrange
        var user = new Domain.DTOs.UserDto
        {
            Name = "Name",
            Email = "email@domain.com"
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/user/", user);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Domain.Entities.User>();

        var getResponse = await _client.GetAsync($"/api/user/{createResult!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getResult = await getResponse.Content.ReadFromJsonAsync<Domain.Entities.User>();

        Assert.NotNull(createResult);
        Assert.NotNull(getResult);
        Assert.Equal(createResult.Id, getResult.Id);
        Assert.Equal("Name", getResult.Name);
        Assert.Equal("email@domain.com", getResult.Email);

        _idsToDelete.Add(createResult.Id);
    }

    [Fact]
    public async Task UserGet_RandomIdShouldNotReturn()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/user/{id}");

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
            await _client.DeleteAsync($"/api/user/{id}");
        }
    }
}
