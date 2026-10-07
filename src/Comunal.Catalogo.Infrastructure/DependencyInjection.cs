using Comunal.Catalogo.Application.Common.Interfaces;
using Comunal.Catalogo.Infrastructure.Persistence;
using Comunal.Catalogo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Comunal.Catalogo.Infrastructure;

/// <summary>
/// Registro de dependencias de la capa de Infraestructura: configura el
/// DbContext (EF Core + SQL Server) y registra los repositorios y la Unit of Work.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CatalogoDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IObjetoRepository, ObjetoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
