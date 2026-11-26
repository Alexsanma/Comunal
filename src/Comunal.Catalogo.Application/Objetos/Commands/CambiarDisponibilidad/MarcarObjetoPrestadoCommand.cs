using Comunal.Catalogo.Application.Common.Exceptions;
using Comunal.Catalogo.Application.Common.Interfaces;
using MediatR;

namespace Comunal.Catalogo.Application.Objetos.Commands.CambiarDisponibilidad;

/// <summary>
/// Command (CQRS): marca un objeto como prestado. Lo invoca de forma síncrona
/// el microservicio Préstamos cuando un Administrador aprueba un préstamo.
/// </summary>
public record MarcarObjetoPrestadoCommand(int Id) : IRequest;

public class MarcarObjetoPrestadoCommandHandler : IRequestHandler<MarcarObjetoPrestadoCommand>
{
    private readonly IObjetoRepository _objetos;
    private readonly IUnitOfWork _unitOfWork;

    public MarcarObjetoPrestadoCommandHandler(IObjetoRepository objetos, IUnitOfWork unitOfWork)
    {
        _objetos = objetos;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MarcarObjetoPrestadoCommand request, CancellationToken cancellationToken)
    {
        var objeto = await _objetos.ObtenerPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el objeto con Id {request.Id}.");

        objeto.MarcarComoPrestado();

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
