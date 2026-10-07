namespace Comunal.Catalogo.Api.Models;

/// <summary>Cuerpo de la petición para actualizar un objeto (el Id viene en la ruta).</summary>
public record ActualizarObjetoRequest(string Nombre, string Descripcion, int CategoriaId);
