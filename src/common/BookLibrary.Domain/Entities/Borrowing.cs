using System.Text.Json.Serialization;

namespace BookLibrary.Domain.Entities;

public class Borrowing
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("bookId")]
    public Guid BookId { get; set; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("borrowDate")]
    public DateTimeOffset? BorrowDate { get; set; }

    [JsonPropertyName("returnDate")]
    public DateTimeOffset? ReturnDate { get; set; }
}
