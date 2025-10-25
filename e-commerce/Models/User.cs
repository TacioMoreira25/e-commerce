using System.ComponentModel.DataAnnotations;

namespace e_commerce.Models;

public class User
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    public TypeRole Role { get; set; } = TypeRole.user;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<Order> Orders { get; set; }
    public ICollection<Address> Addresses { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}

public enum TypeRole
{
    user,
    admin
}