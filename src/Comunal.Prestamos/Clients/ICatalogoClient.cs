namespace Comunal.Prestamos.Clients;

/// <summary>
/// Cliente HTTP del microservicio Catálogo (comunicación síncrona request/response).
/// Reenvía el JWT del usuario que hizo la petición original.
/// </summary>
public interface ICatalogoClient
{
    /// <summary>Consulta un objeto por Id. Devuelve null si el Catálogo responde 404.</summary>
    Task<ObjetoCatalogoDto?> ObtenerObjetoAsync(int objetoId, CancellationToken cancellationToken);

    /// <summary>Lista todos los objetos del catálogo (para mostrar el nombre en los préstamos).</summary>
    Task<IReadOnlyList<ObjetoCatalogoDto>> ObtenerObjetosAsync(CancellationToken cancellationToken);

    /// <summary>Pide al Catálogo marcar el objeto como Prestado.</summary>
    Task MarcarPrestadoAsync(int objetoId, CancellationToken cancellationToken);

    /// <summary>Pide al Catálogo marcar el objeto como Disponible.</summary>
    Task MarcarDisponibleAsync(int objetoId, CancellationToken cancellationToken);
}

/// <summary>Vista mínima de un objeto del Catálogo que necesita Préstamos.</summary>
public record ObjetoCatalogoDto(int Id, string Nombre, string Categoria, string Estado);
