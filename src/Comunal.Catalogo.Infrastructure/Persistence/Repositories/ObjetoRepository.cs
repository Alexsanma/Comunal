using Comunal.Catalogo.Application.Common.Interfaces;
using Comunal.Catalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Comunal.Catalogo.Infrastructure.Persistence.Repositories;

/// <summary>Implementación del repositorio de objetos con EF Core.</summary>
public class ObjetoRepository : IObjetoRepository
{
    private readonly CatalogoDbContext _context;

    public ObjetoRepository(CatalogoDbContext context)
    {
        _context = context;
    }

    public async Task AgregarAsync(Objeto objeto, CancellationToken cancellationToken)
    {
        await _context.Objetos.AddAsync(objeto, cancellationToken);
    }

    public async Task<Objeto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken)
    {
        // Rastreado (sin AsNoTracking) porque los commands lo mutan y lo guardan con la Unit of Work.
        return await _context.Objetos
            .Include(o => o.Categoria)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Objeto>> ObtenerTodosAsync(CancellationToken cancellationToken)
    {
        return await _context.Objetos
            .AsNoTracking()
            .Include(o => o.Categoria)
            .OrderBy(o => o.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Objeto>> ObtenerPorCategoriaAsync(int categoriaId, CancellationToken cancellationToken)
    {
        return await _context.Objetos
            .AsNoTracking()
            .Include(o => o.Categoria)
            .Where(o => o.CategoriaId == categoriaId)
            .OrderBy(o => o.Id)
            .ToListAsync(cancellationToken);
    }
}
