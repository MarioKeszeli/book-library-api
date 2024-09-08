using System.Text.Json.Serialization;

namespace BookLibrary.Domain.DTOs;

public class UserDto
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }
}
