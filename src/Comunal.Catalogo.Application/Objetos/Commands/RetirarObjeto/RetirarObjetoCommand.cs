using Comunal.Catalogo.Application.Common.Exceptions;
using Comunal.Catalogo.Application.Common.Interfaces;
using MediatR;
 
namespace Comunal.Catalogo.Application.Objetos.Commands.RetirarObjeto;
 
/// <summary>Command (CQRS): retira un objeto del catálogo (baja lógica).</summary>
public record RetirarObjetoCommand(int Id) : IRequest;
 
public class RetirarObjetoCommandHandler : IRequestHandler<RetirarObjetoCommand>
{
    private readonly IObjetoRepository _objetos;
    private readonly IUnitOfWork _unitOfWork;
 
    public RetirarObjetoCommandHandler(IObjetoRepository objetos, IUnitOfWork unitOfWork)
    {
        _objetos = objetos;
        _unitOfWork = unitOfWork;
    }
 
    public async Task Handle(RetirarObjetoCommand request, CancellationToken cancellationToken)
    {
        var objeto = await _objetos.ObtenerPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el objeto con Id {request.Id}.");
 
        objeto.Retirar();
 
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
