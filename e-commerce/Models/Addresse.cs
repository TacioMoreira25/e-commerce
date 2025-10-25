namespace e_commerce.Models;

public class Addresse
{
    public Guid Id { get; set; }
    public User UserId { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public int ZipCode { get; set; }
    public string Country { get; set; }
    public string isDefault { get; set; }
}