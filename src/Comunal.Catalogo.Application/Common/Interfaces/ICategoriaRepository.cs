namespace Comunal.Catalogo.Application.Common.Interfaces;

/// <summary>
/// Contrato del repositorio de categorías. Lo usa la capa de Aplicación
/// para verificar que una categoría exista antes de construir el objeto
/// (el dominio da por hecho que la clave foránea es válida).
/// </summary>
public interface ICategoriaRepository
{
    Task<bool> ExisteAsync(int id, CancellationToken cancellationToken);
}
