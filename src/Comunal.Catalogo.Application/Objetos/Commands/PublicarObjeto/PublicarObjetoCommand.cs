using Comunal.Catalogo.Application.Common.Interfaces;
using Comunal.Catalogo.Domain.Entities;
using Comunal.Catalogo.Domain.Exceptions;
using MediatR;
 
namespace Comunal.Catalogo.Application.Objetos.Commands.PublicarObjeto;
 
/// <summary>Command (CQRS): publica un objeto nuevo en el catálogo.</summary>
public record PublicarObjetoCommand(
    string Nombre,
    string Descripcion,
    int CategoriaId,
    Guid PropietarioId) : IRequest<int>;
 
public class PublicarObjetoCommandHandler : IRequestHandler<PublicarObjetoCommand, int>
{
    private readonly IObjetoRepository _objetos;
    private readonly ICategoriaRepository _categorias;
    private readonly IUnitOfWork _unitOfWork;
 
    public PublicarObjetoCommandHandler(
        IObjetoRepository objetos,
        ICategoriaRepository categorias,
        IUnitOfWork unitOfWork)
    {
        _objetos = objetos;
        _categorias = categorias;
        _unitOfWork = unitOfWork;
    }
 
    public async Task<int> Handle(PublicarObjetoCommand request, CancellationToken cancellationToken)
    {
        if (!await _categorias.ExisteAsync(request.CategoriaId, cancellationToken))
            throw new BusinessRuleException("La categoría indicada no existe.");
 
        var objeto = Objeto.Publicar(
            request.Nombre,
            request.Descripcion,
            request.CategoriaId,
            request.PropietarioId);
 
        await _objetos.AgregarAsync(objeto, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);
 
        return objeto.Id;
    }
}
