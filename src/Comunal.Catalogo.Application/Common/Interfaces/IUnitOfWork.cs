namespace Comunal.Catalogo.Application.Common.Interfaces;

/// <summary>
/// Patrón Unit of Work: confirma en una sola transacción todos los cambios
/// hechos a través de los repositorios durante un caso de uso.
/// </summary>
public interface IUnitOfWork
{
    Task<int> GuardarCambiosAsync(CancellationToken cancellationToken);
}
