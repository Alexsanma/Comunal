// Program base del microservicio Catálogo.
// La configuración completa (capas, Swagger, middleware y endpoints) se agrega
// en la capa de API (rama feature/api). Este archivo solo deja el host listo.
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Comunal - Catálogo API (estructura base).");

app.Run();
