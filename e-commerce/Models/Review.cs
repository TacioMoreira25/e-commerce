using System.ComponentModel.DataAnnotations;

namespace e_commerce.Models;

public class Review
{
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public Guid ProductId { get; set; }
    [Required]   
    [Range(1, 5)]
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; }
    public Product Product { get; set; }
}