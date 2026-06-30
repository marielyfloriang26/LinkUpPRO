using LinkUpPro.Domain.Entities;

namespace LinkUpPro.Application.Interfaces.Repositories;

public interface IPublicacionRepository : IRepositoryAsync<Publicacion>
{
    Task<IReadOnlyList<Publicacion>> GetTodasConDetallesAsync();
}