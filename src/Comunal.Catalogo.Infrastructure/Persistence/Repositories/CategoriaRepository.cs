using Comunal.Catalogo.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Comunal.Catalogo.Infrastructure.Persistence.Repositories;

/// <summary>Implementación del repositorio de categorías con EF Core.</summary>
public class CategoriaRepository : ICategoriaRepository
{
    private readonly CatalogoDbContext _context;

    public CategoriaRepository(CatalogoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExisteAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Categorias.AnyAsync(c => c.Id == id, cancellationToken);
    }
}
