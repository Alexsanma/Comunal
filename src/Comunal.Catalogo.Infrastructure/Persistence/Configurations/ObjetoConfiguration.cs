using Comunal.Catalogo.Domain.Entities;
using Comunal.Catalogo.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comunal.Catalogo.Infrastructure.Persistence.Configurations;

public class ObjetoConfiguration : IEntityTypeConfiguration<Objeto>
{
    public void Configure(EntityTypeBuilder<Objeto> builder)
    {
        builder.ToTable("Objetos");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Nombre)
            .IsRequired()
            .HasMaxLength(Objeto.NombreMaximo);

        builder.Property(o => o.Descripcion)
            .IsRequired()
            .HasMaxLength(Objeto.DescripcionMaxima);

        builder.Property(o => o.Estado)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.FechaPublicacion).IsRequired();

        builder.Property(o => o.PropietarioId).IsRequired();

        // Relación Objeto -> Categoria (muchos objetos por categoría).
        builder.HasOne(o => o.Categoria)
            .WithMany(c => c.Objetos)
            .HasForeignKey(o => o.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Datos de ejemplo (semilla). Fecha y propietario fijos para que la migración sea determinista.
        var propietario = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var fecha = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new { Id = 1, Nombre = "Taladro percutor Bosch", Descripcion = "Taladro con maletín y set de brocas.", CategoriaId = 1, Estado = EstadoObjeto.Disponible, FechaPublicacion = fecha, PropietarioId = propietario },
            new { Id = 2, Nombre = "Carpa para 4 personas", Descripcion = "Carpa impermeable, ideal para camping.", CategoriaId = 5, Estado = EstadoObjeto.Disponible, FechaPublicacion = fecha, PropietarioId = propietario },
            new { Id = 3, Nombre = "Cafetera italiana", Descripcion = "Greca de aluminio para 6 tazas.", CategoriaId = 2, Estado = EstadoObjeto.Disponible, FechaPublicacion = fecha, PropietarioId = propietario },
            new { Id = 4, Nombre = "Monitor 24 pulgadas", Descripcion = "Monitor Full HD con cable HDMI.", CategoriaId = 3, Estado = EstadoObjeto.Prestado, FechaPublicacion = fecha, PropietarioId = propietario },
            new { Id = 5, Nombre = "El Hobbit", Descripcion = "Novela de J. R. R. Tolkien, edición de bolsillo.", CategoriaId = 4, Estado = EstadoObjeto.Disponible, FechaPublicacion = fecha, PropietarioId = propietario });
    }
}
