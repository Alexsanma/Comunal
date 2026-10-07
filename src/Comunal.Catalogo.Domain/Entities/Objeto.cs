using Comunal.Catalogo.Domain.Common;
using Comunal.Catalogo.Domain.Enums;
using Comunal.Catalogo.Domain.Exceptions;

namespace Comunal.Catalogo.Domain.Entities;

/// <summary>
/// Raíz de agregado del catálogo: un objeto que un miembro de la comunidad
/// pone a disposición para préstamo. Toda su invariante vive aquí dentro;
/// no se puede construir ni mutar un objeto en un estado inválido.
/// </summary>
public class Objeto : BaseEntity
{
    public const int NombreMinimo = 3;
    public const int NombreMaximo = 100;
    public const int DescripcionMaxima = 500;

    public string Nombre { get; private set; } = default!;
    public string Descripcion { get; private set; } = default!;
    public int CategoriaId { get; private set; }
    public Categoria? Categoria { get; private set; }
    public EstadoObjeto Estado { get; private set; }
    public DateTime FechaPublicacion { get; private set; }
    public Guid PropietarioId { get; private set; }

    // Requerido por EF Core.
    private Objeto() { }

    private Objeto(string nombre, string descripcion, int categoriaId, Guid propietarioId)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        CategoriaId = categoriaId;
        PropietarioId = propietarioId;
        Estado = EstadoObjeto.Disponible;
        FechaPublicacion = DateTime.UtcNow;
    }

    /// <summary>Crea un objeto nuevo y disponible, validando las reglas del dominio.</summary>
    public static Objeto Publicar(string nombre, string descripcion, int categoriaId, Guid propietarioId)
    {
        nombre = ValidarNombre(nombre);
        descripcion = ValidarDescripcion(descripcion);
        ValidarCategoria(categoriaId);
        ValidarPropietario(propietarioId);

        return new Objeto(nombre, descripcion, categoriaId, propietarioId);
    }

    /// <summary>Actualiza los datos básicos del objeto.</summary>
    public void Actualizar(string nombre, string descripcion, int categoriaId)
    {
        if (Estado == EstadoObjeto.Prestado)
            throw new BusinessRuleException("No se puede modificar un objeto que está prestado.");

        Nombre = ValidarNombre(nombre);
        Descripcion = ValidarDescripcion(descripcion);
        ValidarCategoria(categoriaId);
        CategoriaId = categoriaId;
    }

    /// <summary>Retira el objeto del catálogo (baja lógica).</summary>
    public void Retirar()
    {
        if (Estado == EstadoObjeto.Prestado)
            throw new BusinessRuleException("No se puede retirar un objeto que está prestado.");

        if (Estado == EstadoObjeto.Retirado)
            throw new BusinessRuleException("El objeto ya se encuentra retirado.");

        Estado = EstadoObjeto.Retirado;
    }

    public void MarcarComoPrestado()
    {
        if (Estado != EstadoObjeto.Disponible)
            throw new BusinessRuleException("Solo se puede prestar un objeto que está disponible.");

        Estado = EstadoObjeto.Prestado;
    }

    public void MarcarComoDisponible()
    {
        if (Estado != EstadoObjeto.Prestado)
            throw new BusinessRuleException("Solo se puede marcar como disponible un objeto que está prestado.");

        Estado = EstadoObjeto.Disponible;
    }

    private static string ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new BusinessRuleException("El nombre del objeto es requerido.");

        nombre = nombre.Trim();

        if (nombre.Length < NombreMinimo)
            throw new BusinessRuleException($"El nombre debe tener al menos {NombreMinimo} caracteres.");

        if (nombre.Length > NombreMaximo)
            throw new BusinessRuleException($"El nombre no puede superar los {NombreMaximo} caracteres.");

        return nombre;
    }

    private static string ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new BusinessRuleException("La descripción del objeto es requerida.");

        descripcion = descripcion.Trim();

        if (descripcion.Length > DescripcionMaxima)
            throw new BusinessRuleException($"La descripción no puede superar los {DescripcionMaxima} caracteres.");

        return descripcion;
    }

    private static void ValidarCategoria(int categoriaId)
    {
        if (categoriaId <= 0)
            throw new BusinessRuleException("La categoría del objeto es obligatoria.");
    }

    private static void ValidarPropietario(Guid propietarioId)
    {
        if (propietarioId == Guid.Empty)
            throw new BusinessRuleException("El propietario del objeto es obligatorio.");
    }
}
