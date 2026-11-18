namespace Comunal.Prestamos.Domain;

/// <summary>Se lanza cuando se viola una regla de negocio del préstamo (se traduce a HTTP 400).</summary>
public class PrestamoReglaException : Exception
{
    public PrestamoReglaException(string mensaje) : base(mensaje)
    {
    }
}
