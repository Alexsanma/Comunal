using Comunal.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Comunal.Identity.Data;

/// <summary>
/// Crea los roles y usuarios iniciales (Seeder). No hay CRUD de usuarios/roles:
/// los datos base se siembran al iniciar el microservicio.
/// </summary>
public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Administrador", "Miembro" };
        foreach (var rol in roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        await CrearUsuarioAsync(userManager, "admin@comunal.com", "Admin123$", "Administrador del sistema", "Administrador");
        await CrearUsuarioAsync(userManager, "miembro@comunal.com", "Miembro123$", "Miembro de la comunidad", "Miembro");
    }

    private static async Task CrearUsuarioAsync(
        UserManager<ApplicationUser> userManager, string email, string password, string nombre, string rol)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var usuario = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            NombreCompleto = nombre
        };

        // CreateAsync guarda la contraseña con el hash seguro de ASP.NET Core Identity.
        var resultado = await userManager.CreateAsync(usuario, password);
        if (resultado.Succeeded)
            await userManager.AddToRoleAsync(usuario, rol);
    }
}
