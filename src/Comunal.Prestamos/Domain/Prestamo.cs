namespace Comunal.Prestamos.Domain;

/// <summary>
/// Préstamo de un objeto del catálogo a un miembro de la comunidad.
/// El objeto se referencia por su Id (cada microservicio tiene su propia base de datos,
/// por eso no hay clave foránea hacia el Catálogo).
/// </summary>
public class Prestamo
{
    public int Id { get; private set; }
    public int ObjetoId { get; private set; }
    public string SolicitanteId { get; private set; } = default!;
    public DateTime FechaSolicitud { get; private set; }
    public DateTime? FechaAprobacion { get; private set; }
    public DateTime? FechaDevolucion { get; private set; }
    public EstadoPrestamo Estado { get; private set; }

    private Prestamo() { }

    public static Prestamo Solicitar(int objetoId, string solicitanteId)
    {
        if (objetoId <= 0)
            throw new PrestamoReglaException("El objeto del préstamo es obligatorio.");
        if (string.IsNullOrWhiteSpace(solicitanteId))
            throw new PrestamoReglaException("El solicitante del préstamo es obligatorio.");

        return new Prestamo
        {
            ObjetoId = objetoId,
            SolicitanteId = solicitanteId,
            FechaSolicitud = DateTime.UtcNow,
            Estado = EstadoPrestamo.Solicitado
        };
    }

    public void Aprobar()
    {
        if (Estado != EstadoPrestamo.Solicitado)
            throw new PrestamoReglaException("Solo se puede aprobar un préstamo en estado Solicitado.");

        Estado = EstadoPrestamo.Aprobado;
        FechaAprobacion = DateTime.UtcNow;
    }

    public void Devolver()
    {
        if (Estado != EstadoPrestamo.Aprobado)
            throw new PrestamoReglaException("Solo se puede devolver un préstamo que esté Aprobado.");

        Estado = EstadoPrestamo.Devuelto;
        FechaDevolucion = DateTime.UtcNow;
    }
}
