using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Comunal.Identity.Models;
using Comunal.Identity.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Comunal.Identity.Services;

/// <summary>
/// Construye un JWT firmado (HMAC-SHA256) con vigencia definida y los claims
/// que identifican al usuario y sus roles.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _opciones;

    public JwtTokenService(IOptions<JwtOptions> opciones)
    {
        _opciones = opciones.Value;
    }

    public (string Token, DateTime ExpiraEn) GenerarToken(ApplicationUser usuario, IList<string> roles)
    {
        var expiraEn = DateTime.UtcNow.AddMinutes(_opciones.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id),
            new(JwtRegisteredClaimNames.Email, usuario.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, usuario.UserName ?? usuario.Email ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(usuario.NombreCompleto))
            claims.Add(new Claim("nombreCompleto", usuario.NombreCompleto));

        // Un claim de rol por cada rol del usuario: habilita la autorización por roles.
        foreach (var rol in roles)
            claims.Add(new Claim(ClaimTypes.Role, rol));

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opciones.Issuer,
            audience: _opciones.Audience,
            claims: claims,
            expires: expiraEn,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
    }
}
