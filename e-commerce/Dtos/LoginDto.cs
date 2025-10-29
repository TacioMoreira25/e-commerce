using System.ComponentModel.DataAnnotations;

namespace e_commerce.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "Username ou Email é obrigatório")]
    public string UsernameOrEmail { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    public string Password { get; set; } = string.Empty;
}