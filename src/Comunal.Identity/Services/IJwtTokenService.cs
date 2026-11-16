using Comunal.Identity.Models;

namespace Comunal.Identity.Services;

/// <summary>Genera los JSON Web Tokens firmados que emite Identity.</summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiraEn) GenerarToken(ApplicationUser usuario, IList<string> roles);
}
