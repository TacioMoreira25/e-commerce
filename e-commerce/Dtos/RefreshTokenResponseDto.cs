namespace e_commerce.Dtos;

public class RefreshTokenResponseDto
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}