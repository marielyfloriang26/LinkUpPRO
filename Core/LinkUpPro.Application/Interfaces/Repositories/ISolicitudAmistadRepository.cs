using LinkUpPro.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Interfaces.Repositories;

public interface ISolicitudAmistadRepository : IRepositoryAsync<SolicitudAmistad>
{
    Task<IReadOnlyList<SolicitudAmistad>> GetSolicitudesRecibidasAsync(int receptorId);
    Task<IReadOnlyList<SolicitudAmistad>> GetSolicitudesEnviadasAsync(int emisorId);
    Task<SolicitudAmistad> GetSolicitudPendienteAsync(int emisorId, int receptorId);
}
