// Interfaz web de Comunal (Frontend Web). Sirve los archivos estáticos de wwwroot;
// el navegador llama directamente a los microservicios Identity, Catálogo y Préstamos.
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();   // index.html por defecto
app.UseStaticFiles();    // HTML, CSS y JavaScript de wwwroot

app.Run();
