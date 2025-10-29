using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using e_commerce.Data;
using e_commerce.Dtos;
using e_commerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace e_commerce.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ECommerceDbContext _context;

    public AuthService(IConfiguration configuration, ECommerceDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }
    
    public async Task<User?> RegisterAsync(RegisterDto request)
    {
        if(await _context.Users.AnyAsync(u => u.Name == request.Name 
                                              || u.Email == request.Email))
            return null;
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        user.PasswordHash = new PasswordHasher<User>()
            .HashPassword(user, request.Password);
        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
        
    public async Task<TokenResponseDto?> LoginAsync(LoginDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Name == request.UsernameOrEmail || 
                                     u.Email == request.UsernameOrEmail);
        
        if (user == null)
            return null;
        
        var verificationResult = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password);
        
        if (verificationResult == PasswordVerificationResult.Failed)
            return null;
        
        var token = new TokenResponseDto
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
        
        return token;
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenResponseDto request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        
        if(user == null)
            return null;
        
        if(user.RefreshToken != request.RefreshToken || 
           user.RefreshTokenExpiry < DateTime.UtcNow)
            return null;
        
        var token = new TokenResponseDto
        {
            AccessToken = CreateToken(user), 
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
        
        return token;
    }

    public async Task<bool> LogoutAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            return false;
        
        user.RefreshToken = string.Empty;
        user.RefreshTokenExpiry = DateTime.MinValue;
        
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<User?> UpdateAsync(UpdateUserDto request, Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            return null;
        
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id != userId && 
                                     (u.Name == request.Name || u.Email == request.Email));
        
        if (existingUser != null)
            return null;
        
        user.Name = request.Name;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<User?> DeleteUserAsync(DeleteUserDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Name == request.UsernameOrEmail || 
                                     u.Email == request.UsernameOrEmail);
        
        if (user == null)
            return null;
        
        var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == user.Id);
        var hasReviews = await _context.Reviews.AnyAsync(r => r.UserId == user.Id);
        var hasCartItems = await _context.CartItems.AnyAsync(c => c.UserId == user.Id);
        
        if (hasOrders || hasReviews || hasCartItems)
        {
            return null;
        }
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, 
        string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            return false;
        
        var verificationResult = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, currentPassword);
        
        if (verificationResult == PasswordVerificationResult.Failed)
            return false;
        
        user.PasswordHash = new PasswordHasher<User>()
            .HashPassword(user, newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        var refreshToken = Convert.ToBase64String(randomNumber);
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // 7 dias de validade
        
        await _context.SaveChangesAsync();
        
        return refreshToken;
    }
    
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]!));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["AppSettings:Issuer"],
            audience: _configuration["AppSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), 
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}