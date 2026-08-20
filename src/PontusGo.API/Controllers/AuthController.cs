using Microsoft.AspNetCore.Mvc;
using PontusGo.Application.DTOs;
using PontusGo.Application.Interfaces;

namespace PontusGo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.AuthenticateAsync(loginDto);

        if (result == null)
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        var isHttps = Request.IsHttps;
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Impede acesso via JavaScript prevenindo ataques XSS
            Secure = isHttps, // Exige HTTPS caso esteja trafegando em conexão segura
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax, // SameSite=None permite cookies cross-origin (localhost:5173 -> localhost:7242)
            Expires = DateTime.UtcNow.AddDays(7)
        };

        // Anexa o cookie HttpOnly na resposta HTTP
        Response.Cookies.Append("pontusgo_token", result.Token, cookieOptions);

        return Ok(new
        {
            user = result.User
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var isHttps = Request.IsHttps;
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        Response.Cookies.Delete("pontusgo_token", cookieOptions);
        return Ok(new { message = "Logout realizado com sucesso." });
    }
}