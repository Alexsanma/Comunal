using Comunal.Catalogo.Domain.Common;
using Comunal.Catalogo.Domain.Exceptions;

namespace Comunal.Catalogo.Domain.Entities;

/// <summary>
/// Categoría a la que pertenece un objeto (Herramientas, Hogar, etc.).
/// </summary>
public class Categoria : BaseEntity
{
    public string Nombre { get; private set; } = default!;

    public ICollection<Objeto> Objetos { get; private set; } = new List<Objeto>();

    // Requerido por EF Core.
    private Categoria() { }

    public Categoria(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new BusinessRuleException("El nombre de la categoría es requerido.");

        Nombre = nombre.Trim();
    }
}
