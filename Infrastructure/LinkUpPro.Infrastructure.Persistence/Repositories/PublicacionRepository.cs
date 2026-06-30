using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Infrastructure.Persistence.Repositories;

public class PublicacionRepository : RepositoryAsync<Publicacion>, IPublicacionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PublicacionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Publicacion>> GetTodasConDetallesAsync()
    {
        return await _dbContext.Set<Publicacion>()
            .Include(p => p.Usuario) 
            .Include(p => p.Reacciones) 
            .Include(p => p.Comentarios)
                .ThenInclude(c => c.Usuario) 
            .OrderByDescending(p => p.FechaCreacion) 
            .ToListAsync();
    }
}