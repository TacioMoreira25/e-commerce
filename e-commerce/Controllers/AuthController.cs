using System.Security.Claims;
using e_commerce.Dtos;
using e_commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    
    public AuthController(IAuthService service)
    {
        _service = service;
    }
    
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _service.RegisterAsync(request);
        
        if (user == null)
            return BadRequest(new { message = "Usuário já existe!" });

        var response = new UserResponseDto
        {
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
        };
        
        return Ok(response);
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _service.LoginAsync(request);
        
        if (token == null)
            return BadRequest(new { message = "Usuário/Email ou senha inválidos!" });
        
        return Ok(token);
    }
    
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody]
        RefreshTokenResponseDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _service.RefreshTokenAsync(request);
        
        if (token == null)
            return BadRequest(new { message = "Token inválido ou expirado!" });
        
        return Ok(token);
    }
    
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Logout()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { message = "Usuário não identificado!" });

        var userId = Guid.Parse(userIdClaim);
        var result = await _service.LogoutAsync(userId);

        if (!result)
            return NotFound(new { message = "Usuário não encontrado!" });

        return Ok(new { message = "Logout realizado com sucesso!" });
    }
    
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UserResponseDto> GetCurrentUser()
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrEmpty(userName))
            return Unauthorized(new { message = "Usuário não autenticado!" });

        return Ok(new UserResponseDto
        {
            Name = userName,
            Email = userEmail ?? "",
            Role = userRole ?? "user"
        });
    }
}