using System.ComponentModel.DataAnnotations;

namespace e_commerce.Dtos;

public class DeleteUserDto
{
    [Required(ErrorMessage = "Username ou Email é obrigatório")]
    public string UsernameOrEmail { get; set; } = string.Empty;
}