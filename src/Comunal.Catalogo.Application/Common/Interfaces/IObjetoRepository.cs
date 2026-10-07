using Comunal.Catalogo.Domain.Entities;

namespace Comunal.Catalogo.Application.Common.Interfaces;

/// <summary>
/// Contrato del repositorio de objetos (patrón Repository).
/// La capa de Aplicación depende de esta abstracción, no de EF Core.
/// </summary>
public interface IObjetoRepository
{
    Task AgregarAsync(Objeto objeto, CancellationToken cancellationToken);

    Task<Objeto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Objeto>> ObtenerTodosAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Objeto>> ObtenerPorCategoriaAsync(int categoriaId, CancellationToken cancellationToken);
}
