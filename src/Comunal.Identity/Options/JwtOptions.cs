namespace Comunal.Identity.Options;

/// <summary>Configuración del JWT (se lee de la sección "Jwt" de appsettings).</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpiryMinutes { get; set; } = 60;
}
