using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.User;

public class UserUpdate(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _idsToDelete = [];

    [Fact]
    public async Task UserUpdate_EmptyCallReturnsBadRequest()
    {
        // Arrange

        // Act
        var response = await _client.PutAsJsonAsync("/api/user", string.Empty);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UserUpdate_EmptyIdReturnsBadRequest()
    {
        // Arrange
        var user = new Domain.Entities.User
        {
            Id = Guid.Empty
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/user/", user);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UserUpdate_RandomIdShouldNotUpdate()
    {
        // Arrange
        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid()
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/user/", user);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UserUpdate_CreatedUserShouldUpdate()
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

        createResult!.Name = "NameUpdated";
        createResult.Email = "emailupdated@domain.com";

        var updateResponse = await _client.PutAsJsonAsync("/api/user/", createResult);

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updateResult = await updateResponse.Content.ReadFromJsonAsync<Domain.Entities.User>();

        Assert.NotNull(createResult);
        Assert.NotNull(updateResult);
        Assert.Equal(createResult.Id, updateResult.Id);
        Assert.Equal("NameUpdated", updateResult.Name);
        Assert.Equal("emailupdated@domain.com", updateResult.Email);

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
            await _client.DeleteAsync($"/api/user/{id}");
        }
    }
}
