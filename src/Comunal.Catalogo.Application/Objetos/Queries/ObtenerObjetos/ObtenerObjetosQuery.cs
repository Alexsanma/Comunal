using Comunal.Catalogo.Application.Common.Interfaces;
using Comunal.Catalogo.Application.Objetos.Dtos;
using MediatR;
 
namespace Comunal.Catalogo.Application.Objetos.Queries.ObtenerObjetos;
 
/// <summary>Query (CQRS): obtiene el listado completo de objetos del catálogo.</summary>
public record ObtenerObjetosQuery() : IRequest<IReadOnlyList<ObjetoDto>>;
 
public class ObtenerObjetosQueryHandler : IRequestHandler<ObtenerObjetosQuery, IReadOnlyList<ObjetoDto>>
{
    private readonly IObjetoRepository _objetos;
 
    public ObtenerObjetosQueryHandler(IObjetoRepository objetos)
    {
        _objetos = objetos;
    }
 
    public async Task<IReadOnlyList<ObjetoDto>> Handle(ObtenerObjetosQuery request, CancellationToken cancellationToken)
    {
        var objetos = await _objetos.ObtenerTodosAsync(cancellationToken);
 
        return objetos
            .Select(o => new ObjetoDto
            {
                Id = o.Id,
                Nombre = o.Nombre,
                Categoria = o.Categoria!.Nombre,
                Estado = o.Estado.ToString()
            })
            .ToList();
    }
}
