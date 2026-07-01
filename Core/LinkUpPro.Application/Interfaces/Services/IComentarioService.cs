using LinkUpPro.Application.DTOs.ComentarioDTO;

namespace LinkUpPro.Application.Interface.Services;

public interface IComentarioService
{
    Task CrearAsync(CrearComentarioDto dto);
    Task EditarAsync(int id, string contenido, int usuarioId);
    Task EliminarAsync(int id, int usuarioId);
    Task<ComentarioDto?> ObtenerPorIdAsync(int id);
}