using System.Security.Claims;
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
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrestamosController : ControllerBase
{
    private readonly PrestamosContext _context;

    public PrestamosController(PrestamosContext context)
    {
        _context = context;
    }

    /// <summary>Lista todos los préstamos (cualquier usuario autenticado).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PrestamoDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var prestamos = await _context.Prestamos
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);

        return Ok(prestamos.Select(Mapear).ToList());
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

        return Ok(prestamos.Select(Mapear).ToList());
    }

    /// <summary>Solicita un préstamo de un objeto (cualquier usuario autenticado).</summary>
    [HttpPost]
    public async Task<ActionResult> Solicitar([FromBody] CrearPrestamoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var prestamo = Prestamo.Solicitar(request.ObjetoId, ObtenerSolicitante());
            await _context.Prestamos.AddAsync(prestamo, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(ObtenerTodos), new { id = prestamo.Id }, Mapear(prestamo));
        }
        catch (PrestamoReglaException ex)
        {
            return BadRequest(new { status = 400, mensaje = ex.Message });
        }
    }

    /// <summary>Aprueba un préstamo. Solo para el rol Administrador.</summary>
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
            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
        catch (PrestamoReglaException ex)
        {
            return BadRequest(new { status = 400, mensaje = ex.Message });
        }
    }

    /// <summary>Marca un préstamo como devuelto (cualquier usuario autenticado).</summary>
    [HttpPut("{id:int}/devolver")]
    public async Task<ActionResult> Devolver(int id, CancellationToken cancellationToken)
    {
        var prestamo = await _context.Prestamos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (prestamo is null)
            return NotFound();

        try
        {
            prestamo.Devolver();
            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
        catch (PrestamoReglaException ex)
        {
            return BadRequest(new { status = 400, mensaje = ex.Message });
        }
    }

    private string ObtenerSolicitante() =>
        User.FindFirstValue(ClaimTypes.Email)
        ?? User.FindFirstValue("email")
        ?? User.Identity?.Name
        ?? "desconocido";

    private static PrestamoDto Mapear(Prestamo p) =>
        new(p.Id, p.ObjetoId, p.SolicitanteId, p.Estado.ToString(), p.FechaSolicitud, p.FechaDevolucion);
}
