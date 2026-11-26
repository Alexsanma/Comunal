namespace Comunal.Prestamos.Clients;

/// <summary>
/// Error devuelto por el Catálogo (o Catálogo no disponible). Conserva el código HTTP
/// para que Préstamos responda con el mismo significado (400, 403, 404, 503...).
/// </summary>
public class CatalogoException : Exception
{
    public int StatusCode { get; }

    public CatalogoException(int statusCode, string mensaje) : base(mensaje)
    {
        StatusCode = statusCode;
    }
}
