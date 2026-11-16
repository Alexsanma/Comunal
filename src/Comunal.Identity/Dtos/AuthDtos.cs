namespace Comunal.Identity.Dtos;

/// <summary>Credenciales de inicio de sesión.</summary>
public record LoginRequest(string Email, string Password);

/// <summary>Respuesta con el JWT emitido tras una autenticación exitosa.</summary>
public record LoginResponse(string Token, DateTime ExpiraEn, string Email, IList<string> Roles);
