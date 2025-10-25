namespace e_commerce.Models;

public class OrderItems
{
    public int Id { get; set; }
    public String OrderId { get; set; }
    public Guid ProductId { get; set; }
    public String Quantity { get; set; }
    public String Subtotal { get; set; }
}