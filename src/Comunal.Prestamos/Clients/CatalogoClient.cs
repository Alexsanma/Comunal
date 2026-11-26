using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Comunal.Prestamos.Clients;

/// <summary>
/// Implementación con HttpClient tipado. La URL base del Catálogo viene de
/// appsettings.json (Servicios:CatalogoUrl) y en cada llamada se propaga el
/// encabezado Authorization (Bearer JWT) de la petición entrante.
/// </summary>
public class CatalogoClient : ICatalogoClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CatalogoClient> _logger;

    public CatalogoClient(HttpClient http, IHttpContextAccessor httpContextAccessor, ILogger<CatalogoClient> logger)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<ObjetoCatalogoDto?> ObtenerObjetoAsync(int objetoId, CancellationToken cancellationToken)
    {
        using var respuesta = await EnviarAsync(HttpMethod.Get, $"api/objetos/{objetoId}", cancellationToken);

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;

        await AsegurarExitoAsync(respuesta, cancellationToken);
        return await respuesta.Content.ReadFromJsonAsync<ObjetoCatalogoDto>(cancellationToken);
    }

    public async Task<IReadOnlyList<ObjetoCatalogoDto>> ObtenerObjetosAsync(CancellationToken cancellationToken)
    {
        using var respuesta = await EnviarAsync(HttpMethod.Get, "api/objetos", cancellationToken);
        await AsegurarExitoAsync(respuesta, cancellationToken);

        return await respuesta.Content.ReadFromJsonAsync<List<ObjetoCatalogoDto>>(cancellationToken)
               ?? new List<ObjetoCatalogoDto>();
    }

    public async Task MarcarPrestadoAsync(int objetoId, CancellationToken cancellationToken)
    {
        using var respuesta = await EnviarAsync(HttpMethod.Put, $"api/objetos/{objetoId}/prestar", cancellationToken);
        await AsegurarExitoAsync(respuesta, cancellationToken);
    }

    public async Task MarcarDisponibleAsync(int objetoId, CancellationToken cancellationToken)
    {
        using var respuesta = await EnviarAsync(HttpMethod.Put, $"api/objetos/{objetoId}/devolver", cancellationToken);
        await AsegurarExitoAsync(respuesta, cancellationToken);
    }

    private async Task<HttpResponseMessage> EnviarAsync(HttpMethod metodo, string ruta, CancellationToken cancellationToken)
    {
        var peticion = new HttpRequestMessage(metodo, ruta);

        // Propaga el JWT del usuario: el Catálogo vuelve a validar firma, vigencia y rol.
        var autorizacion = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(autorizacion) && AuthenticationHeaderValue.TryParse(autorizacion, out var valor))
            peticion.Headers.Authorization = valor;

        _logger.LogInformation("Préstamos -> Catálogo: {Metodo} {Ruta}", metodo, ruta);

        try
        {
            return await _http.SendAsync(peticion, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "No fue posible comunicarse con el Catálogo.");
            throw new CatalogoException(StatusCodes.Status503ServiceUnavailable,
                "El microservicio Catálogo no está disponible en este momento.");
        }
    }

    private static async Task AsegurarExitoAsync(HttpResponseMessage respuesta, CancellationToken cancellationToken)
    {
        if (respuesta.IsSuccessStatusCode)
            return;

        var mensaje = $"El Catálogo respondió {(int)respuesta.StatusCode}.";
        var cuerpo = await respuesta.Content.ReadAsStringAsync(cancellationToken);

        // El Catálogo devuelve { status, error, message } en sus errores de negocio.
        if (!string.IsNullOrWhiteSpace(cuerpo))
        {
            try
            {
                using var json = JsonDocument.Parse(cuerpo);
                if (json.RootElement.TryGetProperty("message", out var detalle))
                    mensaje = $"Catálogo: {detalle.GetString()}";
            }
            catch (JsonException)
            {
                // Cuerpo no JSON: se conserva el mensaje genérico.
            }
        }

        throw new CatalogoException((int)respuesta.StatusCode, mensaje);
    }
}
