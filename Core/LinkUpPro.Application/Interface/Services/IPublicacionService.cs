
using LinkUpPro.Application.DTOs.Publicacion;
using LinkUpPro.Application.DTOs.PublicacionDTO;

namespace LinkUpPro.Application.Interfaces.Services;

public interface IPublicacionService
{
    Task<List<PublicacionDto>> ObtenerTodasAsync();

    Task CrearAsync(CrearPublicacionDto dto);

    Task EditarAsync(ModificarPublicacionDto dto);
    
    Task EliminarAsync(int id);
}