namespace Comunal.Catalogo.Application.Objetos.Dtos;

/// <summary>DTO resumido para los listados de objetos.</summary>
public class ObjetoDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = default!;
    public string Categoria { get; init; } = default!;
    public string Estado { get; init; } = default!;
}
