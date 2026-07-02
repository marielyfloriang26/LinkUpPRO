using AutoMapper;
using LinkUpPro.Application.DTOs.Notificacion;
using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Domain.Entities;


namespace LinkUpPro.Application.Services;

public class NotificacionService : INotificacionService
{
    private readonly IRepositoryAsync<Notificacion> _notificacionRepository;
    private readonly IMapper _mapper;

    public NotificacionService(IRepositoryAsync<Notificacion> notificacionRepository, IMapper mapper)
    {
        _notificacionRepository = notificacionRepository;
        _mapper = mapper;
    }

    public async Task<List<NotificacionDto>> ObtenerTodasPorUsuarioAsync(int usuarioId)
    {
        var todas = await _notificacionRepository.GetAllAsync();
        
        // Cargar descendente 
        var filtradas = todas
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.FechaNotificacion)
            .ToList();

        return _mapper.Map<List<NotificacionDto>>(filtradas);
    }

    public async Task CrearNotificacionAsync(int usuarioId, int remitenteId, int? publicacionId, string tipoActividad, string mensaje)
    {
        // No generar notificaciones si el remitente es el mismo destinatario de la accion
        if (usuarioId == remitenteId) return;

        var noti = new Notificacion
        {
            UsuarioId = usuarioId,
            RemitenteId = remitenteId,
            PublicacionId = publicacionId,
            TipoActividad = tipoActividad,
            Mensaje = mensaje,
            Leida = false,
            FechaNotificacion = DateTime.UtcNow
        };

        await _notificacionRepository.AddAsync(noti);
    }

    public async Task MarcarComoLeidaAsync(int id, int usuarioId)
    {
        var noti = await _notificacionRepository.GetByIdAsync(id);
        if (noti == null) return;

        // Solo el propietario puede modificar su propia noti
        if (noti.UsuarioId != usuarioId) return;

        noti.Leida = true;
        await _notificacionRepository.UpdateAsync(noti);
    }

    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        var todas = await _notificacionRepository.GetAllAsync();
        var misNotificaciones = todas.Where(n => n.UsuarioId == usuarioId && !n.Leida).ToList();

        foreach (var noti in misNotificaciones)
        {
            noti.Leida = true;
            await _notificacionRepository.UpdateAsync(noti);
        }
    }
}