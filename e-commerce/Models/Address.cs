using System.ComponentModel.DataAnnotations;

namespace e_commerce.Models;

public class Address
{
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
    [Required] 
    public string Street { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string State { get; set; }
    [Required]
    public int ZipCode { get; set; }
    [Required]
    public string Country { get; set; }
    public bool isDefault { get; set; } = false;
    
    public User User { get; set; }
}