using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Domain.Entities;

namespace LinkUpPro.Application.Services;

public class ReaccionService : IReaccionService
{
    private readonly IRepositoryAsync<Reaccion> _reaccionRepository;
    private readonly IPublicacionRepository _publicacionRepository;

    public ReaccionService(
        IRepositoryAsync<Reaccion> reaccionRepository,
        IPublicacionRepository publicacionRepository)
    {
        _reaccionRepository = reaccionRepository;
        _publicacionRepository = publicacionRepository;
    }

    public async Task ReaccionarAsync(int publicacionId, int usuarioId, string tipoReaccion)
    {
        // Valida que la publi exista
        var publicacion = await _publicacionRepository.GetByIdAsync(publicacionId);
        if (publicacion == null || publicacion.EstaEliminada)
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