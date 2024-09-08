using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.User;

public class UserCreate(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _idsToDelete = [];

    [Fact]
    public async Task UserCreate_EmptyCallReturnsBadRequest()
    {
        // Arrange

        // Act
        var response = await _client.PostAsJsonAsync("/api/user", string.Empty);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/user", null, null)]
    [InlineData("/api/user", "", "")]
    [InlineData("/api/user", "   ", "   ")]
    [InlineData("/api/user", "Name", null)]
    [InlineData("/api/user", "Name", "")]
    [InlineData("/api/user", "Name", "   ")]
    [InlineData("/api/user", null, "email@domain.com")]
    [InlineData("/api/user", "", "email@domain.com")]
    [InlineData("/api/user", "   ", "email@domain.com")]
    public async Task UserCreate_ShouldNotCreate(string url, string name, string email)
    {
        // Arrange
        var user = new Domain.DTOs.UserDto
        {
            Name = name,
            Email = email
        };

        // Act
        var response = await _client.PostAsJsonAsync(url, user);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/user", "Name", "email@domain.com")]
    public async Task UserCreate_ShouldCreate(string url, string name, string email)
    {
        // Arrange
        var user = new Domain.DTOs.UserDto
        {
            Name = name,
            Email = email
        };

        // Act
        var response = await _client.PostAsJsonAsync(url, user);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Domain.Entities.User>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(email, result.Email);

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
            await _client.DeleteAsync($"/api/user/{id}");
        }
    }
}
