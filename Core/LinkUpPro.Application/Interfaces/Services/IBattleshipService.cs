using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Interfaces.Services;

public interface IBattleshipService
{
    Task<Response<List<PartidaBattleshipDTO>>> GetPartidasActivasAsync(int usuarioId);
    Task<Response<List<PartidaBattleshipDTO>>> GetHistorialPartidasAsync(int usuarioId);
    Task<Response<int>> IniciarPartidaAsync(int usuario1Id, int usuario2Id);
    Task<Response<bool>> RendirseAsync(int partidaId, int usuarioId);
    Task<Response<bool>> PosicionarBarcoAsync(int usuarioId, PosicionarBarcoDTO dto);
    Task<Response<bool>> AtacarAsync(int usuarioId, AtacarDTO dto);
    Task<Response<List<CasillaTableroDTO>>> GetTableroPropioAsync(int partidaId, int usuarioId);
    Task<Response<List<CasillaTableroDTO>>> GetTableroAtaqueAsync(int partidaId, int usuarioId);
    Task<Response<List<int>>> GetBarcosFaltantesAsync(int partidaId, int usuarioId);
    Task<Response<PartidaBattleshipDTO>> GetPartidaAsync(int partidaId, int usuarioId);
}
