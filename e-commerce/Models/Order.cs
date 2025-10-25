namespace e_commerce.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TotalPrice { get; set; }
    public TypeStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum TypeStatus
{
    pending,
    paid,
    delivered,
    canceled
}