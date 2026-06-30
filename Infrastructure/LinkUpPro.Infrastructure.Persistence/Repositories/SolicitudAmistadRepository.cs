using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories;

public class SolicitudAmistadRepository : RepositoryAsync<SolicitudAmistad>, ISolicitudAmistadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SolicitudAmistadRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SolicitudAmistad>> GetSolicitudesRecibidasAsync(int receptorId)
    {
        return await _dbContext.Set<SolicitudAmistad>()
            .Include(s => s.Emisor)
            .Include(s => s.Receptor)
            .Where(s => s.ReceptorId == receptorId 
                     && s.Estado == "En espera de respuesta" 
                     && s.Emisor.EsActivo 
                     && s.Receptor.EsActivo)
            .OrderByDescending(s => s.FechaEnvio)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<SolicitudAmistad>> GetSolicitudesEnviadasAsync(int emisorId)
    {
        return await _dbContext.Set<SolicitudAmistad>()
            .Include(s => s.Receptor)
            .Include(s => s.Emisor)
            .Where(s => s.EmisorId == emisorId 
                     && (s.Estado == "En espera de respuesta" || s.Estado == "Aceptada" || s.Estado == "Rechazada")
                     && !s.OcultaParaEmisor 
                     && s.Emisor.EsActivo 
                     && s.Receptor.EsActivo)
            .OrderByDescending(s => s.FechaEnvio)
            .ToListAsync();
    }

    public async Task<SolicitudAmistad> GetSolicitudPendienteAsync(int emisorId, int receptorId)
    {
        return await _dbContext.Set<SolicitudAmistad>()
            .FirstOrDefaultAsync(s => s.EmisorId == emisorId && s.ReceptorId == receptorId && s.Estado == "En espera de respuesta");
    }
}
