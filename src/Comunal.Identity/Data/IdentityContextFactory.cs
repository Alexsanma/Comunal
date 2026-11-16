using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Comunal.Identity.Data;

/// <summary>
/// Fábrica en tiempo de diseño: permite que 'dotnet ef migrations' cree el
/// contexto sin levantar la aplicación.
/// </summary>
public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=ComunalIdentity;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new IdentityContext(optionsBuilder.Options);
    }
}
