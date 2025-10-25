using System.ComponentModel.DataAnnotations;

namespace e_commerce.Models;

public class OrderItem
{
    public Guid Id { get; set; }
    [Required]
    public Guid OrderId { get; set; }
    [Required]
    public Guid ProductId { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public decimal Subtotal { get; set; }
    
    public Order Order { get; set; }
    public Product Product { get; set; }
}