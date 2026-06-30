using LinkUpPro.Application.DTOs.ComentarioDTO;

namespace LinkUpPro.Application.Interface.Services;

public interface IComentarioService
{
    Task CrearAsync(CrearComentarioDto dto);
}