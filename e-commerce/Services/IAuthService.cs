using e_commerce.Dtos;
using e_commerce.Models;

namespace e_commerce.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDto request);
    Task<TokenResponseDto?> LoginAsync(UserDto request);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenResponseDto request);
    Task<bool> LogoutAsync(Guid userId);
    Task<User?> UpdateAsync(UserDto request, Guid userId);
    Task<User?> DeleteUserAsync(UserDto request);
}