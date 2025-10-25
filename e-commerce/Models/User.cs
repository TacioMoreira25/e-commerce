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
    public string PasswordHash { get; set; }
    public TypeRole Role { get; set; } = TypeRole.user;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum TypeRole
{
    user,
    admin
}