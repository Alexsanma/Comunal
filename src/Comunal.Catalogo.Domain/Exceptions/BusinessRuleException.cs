namespace Comunal.Catalogo.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se viola una regla de negocio del dominio.
/// La capa de presentación la traduce a un HTTP 400 (Bad Request).
/// </summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string mensaje) : base(mensaje)
    {
    }
}
