using Comunal.Catalogo.Api.Models;
using Comunal.Catalogo.Application.Objetos.Commands.ActualizarObjeto;
using Comunal.Catalogo.Application.Objetos.Commands.PublicarObjeto;
using Comunal.Catalogo.Application.Objetos.Commands.RetirarObjeto;
using Comunal.Catalogo.Application.Objetos.Dtos;
using Comunal.Catalogo.Application.Objetos.Queries.ObtenerObjetoPorId;
using Comunal.Catalogo.Application.Objetos.Queries.ObtenerObjetos;
using Comunal.Catalogo.Application.Objetos.Queries.ObtenerObjetosPorCategoria;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Comunal.Catalogo.Api.Controllers;

/// <summary>
/// API del catálogo de objetos de la comunidad. El controlador no tiene
/// lógica de negocio: delega cada operación en MediatR (patrón Mediator).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ObjetosController : ControllerBase
{
    private readonly ISender _mediator;

    public ObjetosController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Lista todos los objetos del catálogo.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ObjetoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ObjetoDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObtenerObjetosQuery(), cancellationToken);
        return Ok(resultado);
    }

    /// <summary>Obtiene el detalle de un objeto por su Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ObjetoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ObjetoDetalleDto>> ObtenerPorId(int id, CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObtenerObjetoPorIdQuery(id), cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    /// <summary>Lista los objetos de una categoría.</summary>
    [HttpGet("categoria/{categoriaId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<ObjetoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ObjetoDto>>> ObtenerPorCategoria(int categoriaId, CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObtenerObjetosPorCategoriaQuery(categoriaId), cancellationToken);
        return Ok(resultado);
    }

    /// <summary>Publica un objeto nuevo en el catálogo.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Publicar([FromBody] PublicarObjetoCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { id });
    }

    /// <summary>Actualiza los datos de un objeto existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Actualizar(int id, [FromBody] ActualizarObjetoRequest body, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ActualizarObjetoCommand(id, body.Nombre, body.Descripcion, body.CategoriaId),
            cancellationToken);

        return NoContent();
    }

    /// <summary>Retira un objeto del catálogo (baja lógica).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Retirar(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RetirarObjetoCommand(id), cancellationToken);
        return NoContent();
    }
}
