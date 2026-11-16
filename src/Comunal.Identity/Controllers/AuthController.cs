using System.Security.Claims;
using Comunal.Identity.Dtos;
using Comunal.Identity.Models;
using Comunal.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Comunal.Identity.Controllers;

/// <summary>Autenticación: inicio de sesión y emisión de JWT.</summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Autentica con correo y contraseña. Si las credenciales son válidas emite un JWT;
    /// de lo contrario responde 401.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var usuario = await _userManager.FindByEmailAsync(request.Email);

        if (usuario is null || !await _userManager.CheckPasswordAsync(usuario, request.Password))
            return Unauthorized(new { mensaje = "Credenciales inválidas." });

        var roles = await _userManager.GetRolesAsync(usuario);
        var (token, expiraEn) = _jwtTokenService.GenerarToken(usuario, roles);

        return Ok(new LoginResponse(token, expiraEn, usuario.Email!, roles));
    }

    /// <summary>Devuelve los datos del usuario autenticado (sirve para verificar el token).</summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult Me()
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

        return Ok(new { email, roles });
    }
}
