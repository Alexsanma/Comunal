using System.Text.Json;
using Comunal.Catalogo.Application.Common.Exceptions;
using Comunal.Catalogo.Domain.Exceptions;

namespace Comunal.Catalogo.Api.Middleware;

/// <summary>
/// Middleware que traduce las excepciones a respuestas HTTP coherentes:
/// las reglas de negocio del dominio se convierten en 400 (Bad Request),
/// los recursos no encontrados en 404, y cualquier otra en 500.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessRuleException ex)
        {
            await EscribirRespuesta(context, StatusCodes.Status400BadRequest, "Regla de negocio no cumplida", ex.Message);
        }
        catch (NotFoundException ex)
        {
            await EscribirRespuesta(context, StatusCodes.Status404NotFound, "Recurso no encontrado", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado al procesar la petición.");
            await EscribirRespuesta(context, StatusCodes.Status500InternalServerError, "Error interno del servidor", "Ocurrió un error inesperado.");
        }
    }

    private static async Task EscribirRespuesta(HttpContext context, int statusCode, string titulo, string detalle)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var cuerpo = JsonSerializer.Serialize(new
        {
            status = statusCode,
            error = titulo,
            message = detalle
        });

        await context.Response.WriteAsync(cuerpo);
    }
}
