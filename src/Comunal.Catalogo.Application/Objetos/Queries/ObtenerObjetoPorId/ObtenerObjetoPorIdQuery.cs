using Comunal.Catalogo.Application.Common.Interfaces;
using Comunal.Catalogo.Application.Objetos.Dtos;
using MediatR;
 
namespace Comunal.Catalogo.Application.Objetos.Queries.ObtenerObjetoPorId;
 
/// <summary>Query (CQRS): obtiene el detalle de un objeto por su Id.</summary>
public record ObtenerObjetoPorIdQuery(int Id) : IRequest<ObjetoDetalleDto?>;
 
public class ObtenerObjetoPorIdQueryHandler : IRequestHandler<ObtenerObjetoPorIdQuery, ObjetoDetalleDto?>
{
    private readonly IObjetoRepository _objetos;
 
    public ObtenerObjetoPorIdQueryHandler(IObjetoRepository objetos)
    {
        _objetos = objetos;
    }
 
    public async Task<ObjetoDetalleDto?> Handle(ObtenerObjetoPorIdQuery request, CancellationToken cancellationToken)
    {
        var objeto = await _objetos.ObtenerPorIdAsync(request.Id, cancellationToken);
 
        if (objeto is null)
            return null;
 
        return new ObjetoDetalleDto
        {
            Id = objeto.Id,
            Nombre = objeto.Nombre,
            Descripcion = objeto.Descripcion,
            CategoriaId = objeto.CategoriaId,
            Categoria = objeto.Categoria!.Nombre,
            Estado = objeto.Estado.ToString(),
            FechaPublicacion = objeto.FechaPublicacion,
            PropietarioId = objeto.PropietarioId
        };
    }
}
