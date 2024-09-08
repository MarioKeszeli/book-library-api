using System.Net;
using System.Net.Http.Json;

namespace BookLibrary.Api.Controllers.Tests.Borrowing;

public class BorrowingReturn(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

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
        var borrowing = new Domain.DTOs.BorrowingDto
        {
            BookId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
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
}
