using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories;

public class AmistadRepository : RepositoryAsync<Amistad>, IAmistadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AmistadRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Amistad>> GetAmistadesByUsuarioIdAsync(int usuarioId)
    {
        return await _dbContext.Set<Amistad>()
            .Include(a => a.Usuario1)
            .Include(a => a.Usuario2)
            .Where(a => (a.UsuarioId1 == usuarioId || a.UsuarioId2 == usuarioId) && a.Estado == "Activa")
            .ToListAsync();
    }

    public async Task<Amistad> GetAmistadEntreUsuariosAsync(int usuarioId1, int usuarioId2)
    {
        return await _dbContext.Set<Amistad>()
            .FirstOrDefaultAsync(a => ((a.UsuarioId1 == usuarioId1 && a.UsuarioId2 == usuarioId2) ||
                                      (a.UsuarioId1 == usuarioId2 && a.UsuarioId2 == usuarioId1)));
    }
}
