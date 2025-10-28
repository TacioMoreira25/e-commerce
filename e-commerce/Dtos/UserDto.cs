using System.ComponentModel.DataAnnotations;
using e_commerce.Models;

namespace e_commerce.Dtos;

public class UserDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public TypeRole Role { get; set; } = TypeRole.user;
}