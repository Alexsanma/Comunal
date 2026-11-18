using Comunal.Prestamos.Domain;
using Microsoft.EntityFrameworkCore;

namespace Comunal.Prestamos.Data;

/// <summary>DbContext del microservicio Préstamos (su propia base de datos).</summary>
public class PrestamosContext : DbContext
{
    public PrestamosContext(DbContextOptions<PrestamosContext> options) : base(options)
    {
    }

    public DbSet<Prestamo> Prestamos => Set<Prestamo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var prestamo = modelBuilder.Entity<Prestamo>();
        prestamo.ToTable("Prestamos");
        prestamo.HasKey(p => p.Id);
        prestamo.Property(p => p.SolicitanteId).IsRequired().HasMaxLength(256);
        prestamo.Property(p => p.Estado).IsRequired().HasConversion<string>().HasMaxLength(20);

        base.OnModelCreating(modelBuilder);
    }
}
