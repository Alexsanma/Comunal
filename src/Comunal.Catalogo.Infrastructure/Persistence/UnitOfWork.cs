using Comunal.Catalogo.Application.Common.Interfaces;

namespace Comunal.Catalogo.Infrastructure.Persistence;

/// <summary>
/// Implementación del patrón Unit of Work sobre el DbContext de EF Core.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CatalogoDbContext _context;

    public UnitOfWork(CatalogoDbContext context)
    {
        _context = context;
    }

    public Task<int> GuardarCambiosAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
