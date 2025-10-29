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
    private readonly IAuthService _service;
    private readonly ECommerceDbContext _context;
    
    public UserController(IAuthService service, ECommerceDbContext context)
    {
        _service = service;
        _context = context;
    }
    
    [Authorize]
    [HttpPut("update-user")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponseDto>> UpdateUser([FromBody] 
        UpdateUserDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { message = "Usuário não identificado!" });
        
        var userId = Guid.Parse(userIdClaim);
        var user = await _service.UpdateAsync(request, userId);

        if (user == null)
            return BadRequest(new { message = "Nome ou email já está em uso!" });

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
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), 
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    { 
        var users = await _context.Users.ToListAsync();
        
        var response = users.Select(u => new UserResponseDto
        {
            Name = u.Name,
            Email = u.Email,
            Role = u.Role.ToString(),
        }).ToList();
        
        return Ok(response);
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete("delete-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteUser([FromBody] DeleteUserDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _service.DeleteUserAsync(request);
        
        if (response == null)
            return BadRequest(new { message = "Usuário não encontrado" +
                                              " ou possui dependências!" });
        
        return Ok(new { message = "Usuário deletado com sucesso!" });
    }
    
    [Authorize]
    [HttpPut("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { message = "Usuário não identificado!" });
        
        var userId = Guid.Parse(userIdClaim);
        var result = await _service.ChangePasswordAsync(userId, 
            request.CurrentPassword, request.NewPassword);

        if (!result)
            return BadRequest(new { message = "Senha atual incorreta!" });

        return Ok(new { message = "Senha alterada com sucesso!" });
    }
}