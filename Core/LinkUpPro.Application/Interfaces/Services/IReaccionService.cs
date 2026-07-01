
namespace LinkUpPro.Application.Interfaces.Services;

public interface IReaccionService
{
    Task ReaccionarAsync(int publicacionId, int usuarioId, string tipoReaccion);
    Task EliminarReaccionAsync(int publicacionId, int usuarioId);
}