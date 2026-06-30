using LinkUpPro.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Interfaces.Repositories;

public interface IAmistadRepository : IRepositoryAsync<Amistad>
{
    Task<IReadOnlyList<Amistad>> GetAmistadesByUsuarioIdAsync(int usuarioId);
    Task<Amistad> GetAmistadEntreUsuariosAsync(int usuarioId1, int usuarioId2);
}
