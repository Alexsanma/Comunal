using System.Security.Claims;
using Comunal.Prestamos.Clients;
using Comunal.Prestamos.Data;
using Comunal.Prestamos.Domain;
using Comunal.Prestamos.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Comunal.Prestamos.Controllers;

/// <summary>
/// Gestión de préstamos. Todos los endpoints requieren autenticación (JWT válido);
/// la aprobación está restringida al rol Administrador.
/// Se comunica de forma síncrona (HTTP) con el microservicio Catálogo para validar
/// el objeto al solicitarlo y para cambiar su estado al aprobar o devolver.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrestamosController : ControllerBase
{
    private readonly PrestamosContext _context;
    private readonly ICatalogoClient _catalogo;

    public PrestamosController(PrestamosContext context, ICatalogoClient catalogo)
    {
        _context = context;
        _catalogo = catalogo;
    }

    /// <summary>Lista todos los préstamos (cualquier usuario autenticado).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PrestamoDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var prestamos = await _context.Prestamos
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);

        var nombres = await ObtenerNombresObjetosAsync(cancellationToken);
        return Ok(prestamos.Select(p => Mapear(p, nombres.GetValueOrDefault(p.ObjetoId))).ToList());
    }

    /// <summary>Lista los préstamos del usuario autenticado (usa el claim del token).</summary>
    [HttpGet("mios")]
    public async Task<ActionResult<IReadOnlyList<PrestamoDto>>> ObtenerMios(CancellationToken cancellationToken)
    {
        var solicitante = ObtenerSolicitante();

        var prestamos = await _context.Prestamos
            .AsNoTracking()
            .Where(p => p.SolicitanteId == solicitante)
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);

        var nombres = await ObtenerNombresObjetosAsync(cancellationToken);
        return Ok(prestamos.Select(p => Mapear(p, nombres.GetValueOrDefault(p.ObjetoId))).ToList());
    }

    /// <summary>
    /// Solicita un préstamo de un objeto (cualquier usuario autenticado).
    /// Antes de registrarlo consulta al Catálogo que el objeto exista y esté Disponible.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> Solicitar([FromBody] CrearPrestamoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Llamada síncrona: GET Catálogo /api/objetos/{id}
            var objeto = await _catalogo.ObtenerObjetoAsync(request.ObjetoId, cancellationToken);
            if (objeto is null)
                return NotFound(new { status = 404, mensaje = $"El objeto {request.ObjetoId} no existe en el catálogo." });

            if (!string.Equals(objeto.Estado, "Disponible", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { status = 400, mensaje = $"El objeto '{objeto.Nombre}' no está disponible (estado: {objeto.Estado})." });

            var yaSolicitado = await _context.Prestamos.AnyAsync(
                p => p.ObjetoId == request.ObjetoId && p.Estado == EstadoPrestamo.Solicitado, cancellationToken);
            if (yaSolicitado)
                return BadRequest(new { status = 400, mensaje = $"El objeto '{objeto.Nombre}' ya tiene una solicitud pendiente." });

            var prestamo = Prestamo.Solicitar(request.ObjetoId, ObtenerSolicitante());
            await _context.Prestamos.AddAsync(prestamo, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(ObtenerTodos), new { id = prestamo.Id }, Mapear(prestamo, objeto.Nombre));
        }
        catch (PrestamoReglaException ex)
        {
            return BadRequest(new { status = 400, mensaje = ex.Message });
        }
        catch (CatalogoException ex)
        {
            return ErrorCatalogo(ex);
        }
    }

    /// <summary>
    /// Aprueba un préstamo. Solo para el rol Administrador.
    /// Pide al Catálogo marcar el objeto como Prestado antes de guardar.
    /// </summary>
    [HttpPut("{id:int}/aprobar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> Aprobar(int id, CancellationToken cancellationToken)
    {
        var prestamo = await _context.Prestamos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (prestamo is null)
            return NotFound();

        try
        {
            prestamo.Aprobar();

            // Llamada síncrona: PUT Catálogo /api/objetos/{id}/prestar
            await _catalogo.MarcarPrestadoAsync(prestamo.ObjetoId, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
        catch (PrestamoReglaException ex)
        {
            return BadRequest(new { status = 400, mensaje = ex.Message });
        }
        catch (CatalogoException ex)
        {
            return ErrorCatalogo(ex);
        }
    }

    /// <summary>
    /// Marca un préstamo como devuelto (cualquier usuario autenticado).
    /// Pide al Catálogo volver a dejar el objeto Disponible antes de guardar.
    /// </summary>
    [HttpPut("{id:int}/devolver")]
    public async Task<ActionResult> Devolver(int id, CancellationToken cancellationToken)
    {
        var prestamo = await _context.Prestamos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (prestamo is null)
            return NotFound();

        try
        {
            prestamo.Devolver();

            // Llamada síncrona: PUT Catálogo /api/objetos/{id}/devolver
            await _catalogo.MarcarDisponibleAsync(prestamo.ObjetoId, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
        catch (PrestamoReglaException ex)
        {
            return BadRequest(new { status = 400, mensaje = ex.Message });
        }
        catch (CatalogoException ex)
        {
            return ErrorCatalogo(ex);
        }
    }

    /// <summary>Nombres de los objetos (Id → Nombre). Si el Catálogo no responde, se listan sin nombre.</summary>
    private async Task<Dictionary<int, string>> ObtenerNombresObjetosAsync(CancellationToken cancellationToken)
    {
        try
        {
            var objetos = await _catalogo.ObtenerObjetosAsync(cancellationToken);
            return objetos.ToDictionary(o => o.Id, o => o.Nombre);
        }
        catch (CatalogoException)
        {
            return new Dictionary<int, string>();
        }
    }

    private ObjectResult ErrorCatalogo(CatalogoException ex) =>
        StatusCode(ex.StatusCode, new { status = ex.StatusCode, mensaje = ex.Message });

    private string ObtenerSolicitante() =>
        User.FindFirstValue(ClaimTypes.Email)
        ?? User.FindFirstValue("email")
        ?? User.Identity?.Name
        ?? "desconocido";

    private static PrestamoDto Mapear(Prestamo p, string? nombreObjeto) =>
        new(p.Id, p.ObjetoId, nombreObjeto, p.SolicitanteId, p.Estado.ToString(), p.FechaSolicitud, p.FechaDevolucion);
}
