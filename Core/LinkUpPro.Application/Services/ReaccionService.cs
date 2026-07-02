using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Domain.Entities;

namespace LinkUpPro.Application.Services;

public class ReaccionService : IReaccionService
{
    private readonly IRepositoryAsync<Reaccion> _reaccionRepository;
    private readonly IPublicacionRepository _publicacionRepository;
    private readonly IRepositoryAsync<Usuario> _usuarioRepository;
    private readonly INotificacionService _notificacionService;

    public ReaccionService(
        IRepositoryAsync<Reaccion> reaccionRepository,
        IPublicacionRepository publicacionRepository, INotificacionService notificacionService,IRepositoryAsync<Usuario> usuarioRepository)
    {
        _reaccionRepository = reaccionRepository;
        _publicacionRepository = publicacionRepository;
        _notificacionService = notificacionService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task ReaccionarAsync(int publicacionId, int usuarioId, string tipoReaccion)
    {
        // Valida que la publi exista
        var publicacion = await _publicacionRepository.GetByIdAsync(publicacionId);
        if (publicacion == null || publicacion.Estado == "Eliminada")
        {
            throw new Exception("La publicación no existe o no se encuentra activa.");
        }

        // Busca si el usuario ya reacciono a este post
        var todasReacciones = await _reaccionRepository.GetAllAsync();
        var reaccionExistente = todasReacciones.FirstOrDefault(r => r.PublicacionId == publicacionId && r.UsuarioId == usuarioId);

        if (reaccionExistente != null)
        {
            // Si hace clic en la misma reaccion, no hace nada o la mantiene
            // Si hace clic en la contraria, actualiza el tipo sin duplicar
            if (reaccionExistente.TipoReaccion != tipoReaccion)
            {
                reaccionExistente.TipoReaccion = tipoReaccion;
                await _reaccionRepository.UpdateAsync(reaccionExistente);

                // Notificar cambio de reacción
                if (publicacion.UsuarioId != usuarioId)
                {
                    var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                    var reaccionTxt = tipoReaccion == "Like" ? "Me gusta" : "No me gusta";
                    var msg = $"{usuario.UserName} reaccionó con {reaccionTxt} a tu publicación.";
                    await _notificacionService.CrearNotificacionAsync(
                        publicacion.UsuarioId, 
                        usuarioId, 
                        publicacionId, 
                        "Reaccion", 
                        msg);
                }
            }
        }
        else
        {
            // Si no existe, crea una nueva
            var nuevaReaccion = new Reaccion
            {
                PublicacionId = publicacionId,
                UsuarioId = usuarioId,
                TipoReaccion = tipoReaccion
            };
            await _reaccionRepository.AddAsync(nuevaReaccion);

            // Notificar nueva reacción
            if (publicacion.UsuarioId != usuarioId)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                var reaccionTxt = tipoReaccion == "Like" ? "Me gusta" : "No me gusta";
                var msg = $"{usuario.UserName} reaccionó con {reaccionTxt} a tu publicación.";
                await _notificacionService.CrearNotificacionAsync(
                    publicacion.UsuarioId, 
                    usuarioId, 
                    publicacionId, 
                    "Reaccion", 
                    msg);
            }
        }
    }

    public async Task EliminarReaccionAsync(int publicacionId, int usuarioId)
    {
        var todasReacciones = await _reaccionRepository.GetAllAsync();
        var reaccionExistente = todasReacciones.FirstOrDefault(r => r.PublicacionId == publicacionId && r.UsuarioId == usuarioId);

        if (reaccionExistente != null)
        {
            await _reaccionRepository.DeleteAsync(reaccionExistente);
        }
    }
}