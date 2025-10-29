using e_commerce.Dtos;
using e_commerce.Models;

namespace e_commerce.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterDto request);
    Task<TokenResponseDto?> LoginAsync(LoginDto request);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenResponseDto request);
    Task<bool> LogoutAsync(Guid userId);
    Task<User?> UpdateAsync(UpdateUserDto request, Guid userId);
    Task<User?> DeleteUserAsync(DeleteUserDto request);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}