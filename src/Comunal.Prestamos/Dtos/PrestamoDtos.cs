namespace Comunal.Prestamos.Dtos;

/// <summary>Cuerpo para solicitar un préstamo (el solicitante se toma del JWT).</summary>
public record CrearPrestamoRequest(int ObjetoId);

/// <summary>Representación de un préstamo para las respuestas de la API.</summary>
public record PrestamoDto(
    int Id,
    int ObjetoId,
    string SolicitanteId,
    string Estado,
    DateTime FechaSolicitud,
    DateTime? FechaDevolucion);
