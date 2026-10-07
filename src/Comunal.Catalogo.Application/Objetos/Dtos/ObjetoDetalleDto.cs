namespace Comunal.Catalogo.Application.Objetos.Dtos;

/// <summary>DTO detallado para la consulta de un objeto por su Id.</summary>
public class ObjetoDetalleDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = default!;
    public string Descripcion { get; init; } = default!;
    public int CategoriaId { get; init; }
    public string Categoria { get; init; } = default!;
    public string Estado { get; init; } = default!;
    public DateTime FechaPublicacion { get; init; }
    public Guid PropietarioId { get; init; }
}
