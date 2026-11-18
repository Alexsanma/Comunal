namespace Comunal.Prestamos.Options;

/// <summary>Configuración del JWT que este microservicio valida (sección "Jwt").</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpiryMinutes { get; set; } = 60;
}
