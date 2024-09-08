using System.Text.Json.Serialization;

namespace BookLibrary.Domain.DTOs;

public class BorrowingDto
{
    [JsonPropertyName("bookId")]
    public required Guid BookId { get; set; }

    [JsonPropertyName("userId")]
    public required Guid UserId { get; set; }

    [JsonPropertyName("borrowDate")]
    public required DateTimeOffset BorrowDate { get; set; }

    [JsonPropertyName("returnDate")]
    public required DateTimeOffset ReturnDate { get; set; }
}
