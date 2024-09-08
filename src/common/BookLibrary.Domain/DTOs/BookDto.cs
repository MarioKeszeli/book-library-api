using System.Text.Json.Serialization;

namespace BookLibrary.Domain.DTOs;

public class BookDto
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("author")]
    public required string Author { get; set; }
}
