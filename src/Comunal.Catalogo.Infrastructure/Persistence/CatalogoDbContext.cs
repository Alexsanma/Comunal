using Comunal.Catalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Comunal.Catalogo.Infrastructure.Persistence;

/// <summary>
/// Contexto de EF Core del microservicio Catálogo. Cada microservicio
/// es dueño de su propia base de datos (no se comparte con otros servicios).
/// </summary>
public class CatalogoDbContext : DbContext
{
    public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options) : base(options)
    {
    }

    public DbSet<Objeto> Objetos => Set<Objeto>();

    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
