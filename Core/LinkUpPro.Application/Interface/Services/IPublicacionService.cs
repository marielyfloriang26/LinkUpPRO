
using LinkUpPro.Application.DTOs.Publicacion;
using LinkUpPro.Application.DTOs.PublicacionDTO;

namespace LinkUpPro.Application.Interfaces.Services;

public interface IPublicacionService
{
    Task<List<PublicacionDto>> ObtenerTodasAsync(int currentUserId, string? textoBusqueda = null, string? tipoContenido = null, string? estadoEdicion = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null);

    Task<List<PublicacionDto>> ObtenerPublicacionesAmigosAsync(int currentUserId, string? textoBusqueda = null, int? amigoId = null, string? tipoContenido = null, string? estadoEdicion = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
    Task CrearAsync(CrearPublicacionDto dto);

    Task EditarAsync(ModificarPublicacionDto dto);

    Task EliminarAsync(int id);
}