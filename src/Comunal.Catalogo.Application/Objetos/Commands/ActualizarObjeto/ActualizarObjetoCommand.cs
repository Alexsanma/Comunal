using Comunal.Catalogo.Application.Common.Exceptions;
using Comunal.Catalogo.Application.Common.Interfaces;
using Comunal.Catalogo.Domain.Exceptions;
using MediatR;
 
namespace Comunal.Catalogo.Application.Objetos.Commands.ActualizarObjeto;
 
/// <summary>Command (CQRS): actualiza los datos básicos de un objeto.</summary>
public record ActualizarObjetoCommand(
    int Id,
    string Nombre,
    string Descripcion,
    int CategoriaId) : IRequest;
 
public class ActualizarObjetoCommandHandler : IRequestHandler<ActualizarObjetoCommand>
{
    private readonly IObjetoRepository _objetos;
    private readonly ICategoriaRepository _categorias;
    private readonly IUnitOfWork _unitOfWork;
 
    public ActualizarObjetoCommandHandler(
        IObjetoRepository objetos,
        ICategoriaRepository categorias,
        IUnitOfWork unitOfWork)
    {
        _objetos = objetos;
        _categorias = categorias;
        _unitOfWork = unitOfWork;
    }
 
    public async Task Handle(ActualizarObjetoCommand request, CancellationToken cancellationToken)
    {
        var objeto = await _objetos.ObtenerPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el objeto con Id {request.Id}.");
 
        if (!await _categorias.ExisteAsync(request.CategoriaId, cancellationToken))
            throw new BusinessRuleException("La categoría indicada no existe.");
 
        objeto.Actualizar(request.Nombre, request.Descripcion, request.CategoriaId);
 
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
