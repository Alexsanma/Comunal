using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Comunal.Catalogo.Application;

/// <summary>
/// Registro de dependencias de la capa de Aplicación. Registra MediatR
/// (patrón Mediator) y descubre automáticamente todos los handlers de
/// commands y queries (CQRS) de este ensamblado.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
