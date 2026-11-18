using Comunal.Prestamos.Domain;
using Microsoft.EntityFrameworkCore;

namespace Comunal.Prestamos.Data;

/// <summary>Siembra un par de préstamos de ejemplo para poder consultar desde el primer momento.</summary>
public static class PrestamosSeeder
{
    public static async Task SeedAsync(PrestamosContext context)
    {
        if (await context.Prestamos.AnyAsync())
            return;

        var solicitado = Prestamo.Solicitar(objetoId: 1, solicitanteId: "miembro@comunal.com");

        var aprobado = Prestamo.Solicitar(objetoId: 4, solicitanteId: "miembro@comunal.com");
        aprobado.Aprobar();

        await context.Prestamos.AddRangeAsync(solicitado, aprobado);
        await context.SaveChangesAsync();
    }
}
