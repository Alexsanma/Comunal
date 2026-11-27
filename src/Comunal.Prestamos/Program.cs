using System.Text;
using Comunal.Prestamos.Clients;
using Comunal.Prestamos.Data;
using Comunal.Prestamos.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Configuración del JWT (debe coincidir con la de Identity) ---
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

// --- Base de datos ---
builder.Services.AddDbContext<PrestamosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Comunicación síncrona con el microservicio Catálogo (HttpClient tipado) ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ICatalogoClient, CatalogoClient>(cliente =>
{
    cliente.BaseAddress = new Uri(builder.Configuration["Servicios:CatalogoUrl"]!);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

// --- Validación del JWT emitido por Identity (firma, emisor, audiencia y vigencia) ---
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
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Comunal - Préstamos API", Version = "v1" });

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

// Aplica migraciones y siembra préstamos de ejemplo.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PrestamosContext>();
    context.Database.Migrate();
    await PrestamosSeeder.SeedAsync(context);
}

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
