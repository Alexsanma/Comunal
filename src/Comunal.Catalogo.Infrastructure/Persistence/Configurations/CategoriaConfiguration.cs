using Comunal.Catalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comunal.Catalogo.Infrastructure.Persistence.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(80);

        builder.HasIndex(c => c.Nombre).IsUnique();

        // Datos de ejemplo (semilla).
        builder.HasData(
            new { Id = 1, Nombre = "Herramientas" },
            new { Id = 2, Nombre = "Hogar" },
            new { Id = 3, Nombre = "Electrónica" },
            new { Id = 4, Nombre = "Libros" },
            new { Id = 5, Nombre = "Deportes" });
    }
}
