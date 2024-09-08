using System.Text.Json.Serialization;

namespace BookLibrary.Domain.Entities;

public class Book
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("borrowed")]
    public bool Borrowed { get; set; }
}
