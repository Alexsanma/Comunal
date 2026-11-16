using Comunal.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Comunal.Identity.Data;

/// <summary>
/// DbContext del microservicio Identity. Hereda de IdentityDbContext, que crea
/// las tablas de usuarios, roles y la relación entre ellos.
/// </summary>
public class IdentityContext : IdentityDbContext<ApplicationUser>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }
}
