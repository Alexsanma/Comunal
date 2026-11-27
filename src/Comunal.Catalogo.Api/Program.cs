using System.Text;
using Comunal.Catalogo.Api.Middleware;
using Comunal.Catalogo.Application;
using Comunal.Catalogo.Infrastructure;
using Comunal.Catalogo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Registro de capas (Clean Architecture) ---
builder.Services.AddApplication();                         // MediatR + casos de uso (CQRS)
builder.Services.AddInfrastructure(builder.Configuration); // EF Core + SQL Server, repositorios y Unit of Work

// --- Validación del JWT emitido por Identity (firma, emisor, audiencia y vigencia) ---
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// --- CORS: permite que la interfaz web (Comunal.Web) llame a este microservicio ---
builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", politica => politica
        .WithOrigins(builder.Configuration["Frontend:Url"] ?? "http://localhost:5080")
        .AllowAnyHeader()
        .AllowAnyMethod()));

// --- Servicios web ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Comunal - Catálogo API",
        Version = "v1",
        Description = "Microservicio de catálogo de objetos de la comunidad (Clean Architecture + DDD + CQRS)."
    });

    var esquema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega aquí el JWT emitido por Identity (sin la palabra 'Bearer').",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    options.AddSecurityDefinition("Bearer", esquema);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [esquema] = Array.Empty<string>() });
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
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
