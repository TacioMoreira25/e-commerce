using System.Security.Claims;
using e_commerce.Data;
using e_commerce.Dtos;
using e_commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_commerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IAuthService service;
    private readonly ECommerceDbContext context;
    
    public UserController(IAuthService service, ECommerceDbContext context)
    {
        this.service = service;
        this.context = context;
    }
    
    [Authorize]
    [HttpPut("update-user")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(UserDto request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("Usuário não identificado!");
        
        var userId = Guid.Parse(userIdClaim);
        var user = await service.UpdateAsync(request, userId);

        var response = new UserResponseDto
        {
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
        };
        return Ok(response);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("get-users")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    { 
        var user = await context.Users.ToListAsync();
        
        var response = user.Select(u => new UserResponseDto
        {
            Name = u.Name,
            Email = u.Email,
            Role = u.Role.ToString(),
        }).ToList();
        
        return Ok(response);
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete("delete-users")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> DeleteUsers(UserDto request)
    {
        var response = await service.DeleteUserAsync(request);
        if (response is null)
            return BadRequest("invalid username");
        
        return Ok("Usuario deletado com sucesso!");
    }
}