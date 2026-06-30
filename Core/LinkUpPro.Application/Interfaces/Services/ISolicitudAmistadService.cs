using LinkUpPro.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Interfaces.Services;

public interface ISolicitudAmistadService
{
    Task SendSolicitudAsync(int emisorId, int receptorId);
    Task AcceptSolicitudAsync(int solicitudId, int receptorId);
    Task RejectSolicitudAsync(int solicitudId, int receptorId);
    Task CancelSolicitudAsync(int solicitudId, int emisorId);
    Task RemoveSolicitudFromHistoryAsync(int solicitudId, int usuarioId);
    Task<IReadOnlyList<SolicitudAmistadDto>> GetSolicitudesRecibidasAsync(int receptorId);
    Task<IReadOnlyList<SolicitudAmistadDto>> GetSolicitudesEnviadasAsync(int emisorId);
    Task<IReadOnlyList<UsuarioDto>> BuscarUsuariosParaAgregarAsync(int currentUserId, string searchString);
}
