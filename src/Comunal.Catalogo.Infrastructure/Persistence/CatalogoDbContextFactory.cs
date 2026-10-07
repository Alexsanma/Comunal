using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Comunal.Catalogo.Infrastructure.Persistence;

/// <summary>
/// Fábrica en tiempo de diseño. Permite que los comandos de EF Core
/// ('dotnet ef migrations' / 'database update') creen el DbContext sin
/// necesitar levantar la API, usando la cadena de conexión de LocalDB.
/// </summary>
public class CatalogoDbContextFactory : IDesignTimeDbContextFactory<CatalogoDbContext>
{
    public CatalogoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogoDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=ComunalCatalogo;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new CatalogoDbContext(optionsBuilder.Options);
    }
}
