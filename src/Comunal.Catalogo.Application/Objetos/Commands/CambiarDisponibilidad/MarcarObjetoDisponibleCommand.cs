using Comunal.Catalogo.Application.Common.Exceptions;
using Comunal.Catalogo.Application.Common.Interfaces;
using MediatR;

namespace Comunal.Catalogo.Application.Objetos.Commands.CambiarDisponibilidad;

/// <summary>
/// Command (CQRS): vuelve a dejar disponible un objeto prestado. Lo invoca de forma
/// síncrona el microservicio Préstamos cuando se registra la devolución.
/// </summary>
public record MarcarObjetoDisponibleCommand(int Id) : IRequest;

public class MarcarObjetoDisponibleCommandHandler : IRequestHandler<MarcarObjetoDisponibleCommand>
{
    private readonly IObjetoRepository _objetos;
    private readonly IUnitOfWork _unitOfWork;

    public MarcarObjetoDisponibleCommandHandler(IObjetoRepository objetos, IUnitOfWork unitOfWork)
    {
        _objetos = objetos;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MarcarObjetoDisponibleCommand request, CancellationToken cancellationToken)
    {
        var objeto = await _objetos.ObtenerPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el objeto con Id {request.Id}.");

        objeto.MarcarComoDisponible();

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
