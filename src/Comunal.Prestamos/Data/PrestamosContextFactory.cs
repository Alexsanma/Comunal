using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Comunal.Prestamos.Data;

/// <summary>Fábrica en tiempo de diseño para los comandos de 'dotnet ef'.</summary>
public class PrestamosContextFactory : IDesignTimeDbContextFactory<PrestamosContext>
{
    public PrestamosContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PrestamosContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=ComunalPrestamos;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new PrestamosContext(optionsBuilder.Options);
    }
}
