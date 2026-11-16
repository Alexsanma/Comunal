using Microsoft.AspNetCore.Identity;

namespace Comunal.Identity.Models;

/// <summary>
/// Usuario de la aplicación. Extiende IdentityUser de ASP.NET Core Identity,
/// que ya gestiona el hash seguro de la contraseña (nunca se guarda en texto plano).
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? NombreCompleto { get; set; }
}
