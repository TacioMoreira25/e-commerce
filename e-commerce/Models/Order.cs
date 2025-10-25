using System.ComponentModel.DataAnnotations;

namespace e_commerce.Models;

public class Order
{
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public decimal TotalPrice { get; set; }
    [Required]
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public User User { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }

}

public enum OrderStatus
{
    pending,
    paid,
    delivered,
    canceled
}