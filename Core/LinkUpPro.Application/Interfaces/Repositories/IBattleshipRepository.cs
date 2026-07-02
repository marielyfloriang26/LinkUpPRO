using LinkUpPro.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Interfaces.Repositories;

public interface IBattleshipRepository : IRepositoryAsync<PartidaBattleship>
{
    Task<List<PartidaBattleship>> GetPartidasActivasByUsuarioIdAsync(int usuarioId);
    Task<List<PartidaBattleship>> GetHistorialPartidasByUsuarioIdAsync(int usuarioId);
    Task<PartidaBattleship?> GetPartidaActivaEntreUsuariosAsync(int usuario1Id, int usuario2Id);
    Task<PartidaBattleship?> GetPartidaByIdWithIncludesAsync(int partidaId);
    Task AddCasillaAsync(CasillaTablero casilla);
    Task AddCasillasAsync(IEnumerable<CasillaTablero> casillas);
    Task UpdateCasillaAsync(CasillaTablero casilla);
    Task<List<CasillaTablero>> GetCasillasByPartidaAndUsuarioAsync(int partidaId, int usuarioId);
    Task<CasillaTablero?> GetCasillaAsync(int partidaId, int usuarioId, int fila, int columna);
}
