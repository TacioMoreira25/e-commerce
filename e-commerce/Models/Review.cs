using System.ComponentModel.DataAnnotations;

namespace e_commerce.Models;

public class Review
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}