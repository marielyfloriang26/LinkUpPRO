using LinkUpPro.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Interfaces.Services;

public interface IAmigoService
{
    Task<IReadOnlyList<UsuarioDto>> GetAmigosAsync(int usuarioId);
    Task<IReadOnlyList<UsuarioDto>> BuscarAmigosAsync(int usuarioId, string searchString);
    Task<int> GetAmigosEnComunCountAsync(int usuarioId1, int usuarioId2);
    Task DeleteAmigoAsync(int usuarioId, int amigoId);
}
