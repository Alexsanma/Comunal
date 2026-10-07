using Comunal.Catalogo.Api.Middleware;
using Comunal.Catalogo.Application;
using Comunal.Catalogo.Infrastructure;
using Comunal.Catalogo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Registro de capas (Clean Architecture) ---
builder.Services.AddApplication();                         // MediatR + casos de uso (CQRS)
builder.Services.AddInfrastructure(builder.Configuration); // EF Core + SQL Server, repositorios y Unit of Work

// --- Servicios web ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Comunal - Catálogo API",
        Version = "v1",
        Description = "Microservicio de catálogo de objetos de la comunidad (Clean Architecture + DDD + CQRS)."
    });
});

var app = builder.Build();

// Aplica las migraciones pendientes y crea/siembra la base de datos al iniciar.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CatalogoDbContext>();
    context.Database.Migrate();
}

// Traduce las excepciones de dominio/aplicación a respuestas HTTP.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
