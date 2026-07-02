using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories;

public class BattleshipRepository : RepositoryAsync<PartidaBattleship>, IBattleshipRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BattleshipRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PartidaBattleship>> GetPartidasActivasByUsuarioIdAsync(int usuarioId)
    {
        return await _dbContext.PartidasBattleship
            .Include(p => p.Jugador1)
            .Include(p => p.Jugador2)
            .Include(p => p.Ganador)
            .Where(p => (p.Jugador1Id == usuarioId || p.Jugador2Id == usuarioId) && p.EstadoPartida != "Finalizada")
            .ToListAsync();
    }

    public async Task<List<PartidaBattleship>> GetHistorialPartidasByUsuarioIdAsync(int usuarioId)
    {
        return await _dbContext.PartidasBattleship
            .Include(p => p.Jugador1)
            .Include(p => p.Jugador2)
            .Include(p => p.Ganador)
            .Where(p => (p.Jugador1Id == usuarioId || p.Jugador2Id == usuarioId) && p.EstadoPartida == "Finalizada")
            .ToListAsync();
    }

    public async Task<PartidaBattleship?> GetPartidaActivaEntreUsuariosAsync(int usuario1Id, int usuario2Id)
    {
        return await _dbContext.PartidasBattleship
            .Where(p => p.EstadoPartida != "Finalizada" && 
                        ((p.Jugador1Id == usuario1Id && p.Jugador2Id == usuario2Id) || 
                         (p.Jugador1Id == usuario2Id && p.Jugador2Id == usuario1Id)))
            .FirstOrDefaultAsync();
    }

    public async Task<PartidaBattleship?> GetPartidaByIdWithIncludesAsync(int partidaId)
    {
        return await _dbContext.PartidasBattleship
            .Include(p => p.Jugador1)
            .Include(p => p.Jugador2)
            .Include(p => p.TurnoUsuario)
            .Include(p => p.Casillas)
            .FirstOrDefaultAsync(p => p.Id == partidaId);
    }

    public async Task AddCasillaAsync(CasillaTablero casilla)
    {
        await _dbContext.CasillasTablero.AddAsync(casilla);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCasillasAsync(IEnumerable<CasillaTablero> casillas)
    {
        await _dbContext.CasillasTablero.AddRangeAsync(casillas);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCasillaAsync(CasillaTablero casilla)
    {
        _dbContext.CasillasTablero.Update(casilla);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<CasillaTablero>> GetCasillasByPartidaAndUsuarioAsync(int partidaId, int usuarioId)
    {
        return await _dbContext.CasillasTablero
            .Where(c => c.PartidaId == partidaId && c.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task<CasillaTablero?> GetCasillaAsync(int partidaId, int usuarioId, int fila, int columna)
    {
        return await _dbContext.CasillasTablero
            .FirstOrDefaultAsync(c => c.PartidaId == partidaId && c.UsuarioId == usuarioId && c.Fila == fila && c.Columna == columna);
    }
}
