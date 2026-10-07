using Comunal.Catalogo.Application.Common.Interfaces;
using Comunal.Catalogo.Application.Objetos.Dtos;
using MediatR;
 
namespace Comunal.Catalogo.Application.Objetos.Queries.ObtenerObjetosPorCategoria;
 
/// <summary>Query (CQRS): obtiene los objetos que pertenecen a una categoría.</summary>
public record ObtenerObjetosPorCategoriaQuery(int CategoriaId) : IRequest<IReadOnlyList<ObjetoDto>>;
 
public class ObtenerObjetosPorCategoriaQueryHandler
    : IRequestHandler<ObtenerObjetosPorCategoriaQuery, IReadOnlyList<ObjetoDto>>
{
    private readonly IObjetoRepository _objetos;
 
    public ObtenerObjetosPorCategoriaQueryHandler(IObjetoRepository objetos)
    {
        _objetos = objetos;
    }
 
    public async Task<IReadOnlyList<ObjetoDto>> Handle(
        ObtenerObjetosPorCategoriaQuery request,
        CancellationToken cancellationToken)
    {
        var objetos = await _objetos.ObtenerPorCategoriaAsync(request.CategoriaId, cancellationToken);
 
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
