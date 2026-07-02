using LinkUpPro.Application.DTOs.Notificacion;


namespace LinkUpPro.Application.Interfaces.Services;

public interface INotificacionService
{
    Task<List<NotificacionDto>> ObtenerTodasPorUsuarioAsync(int usuarioId);
    Task CrearNotificacionAsync(int usuarioId, int remitenteId, int? publicacionId, string tipoActividad, string mensaje);
    Task MarcarComoLeidaAsync(int id, int usuarioId);
    Task MarcarTodasComoLeidasAsync(int usuarioId);
}