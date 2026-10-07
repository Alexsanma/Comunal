namespace Comunal.Catalogo.Application.Common.Exceptions;

/// <summary>
/// Se lanza cuando un caso de uso no encuentra el recurso solicitado.
/// La capa de presentación la traduce a un HTTP 404 (Not Found).
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string mensaje) : base(mensaje)
    {
    }
}
