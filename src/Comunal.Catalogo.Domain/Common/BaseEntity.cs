namespace Comunal.Catalogo.Domain.Common;

/// <summary>
/// Clase base para las entidades del dominio. Expone el identificador
/// con "set" protegido para que solo la propia entidad controle su estado.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }
}
