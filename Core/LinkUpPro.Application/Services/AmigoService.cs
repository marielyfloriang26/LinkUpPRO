using LinkUpPro.Application.DTOs;
using LinkUpPro.Application.Exceptions;
using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Services;

public class AmigoService : IAmigoService
{
    private readonly IAmistadRepository _amistadRepository;
    private readonly IRepositoryAsync<Domain.Entities.Usuario> _usuarioRepository;

    public AmigoService(IAmistadRepository amistadRepository, IRepositoryAsync<Domain.Entities.Usuario> usuarioRepository)
    {
        _amistadRepository = amistadRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IReadOnlyList<UsuarioDto>> GetAmigosAsync(int usuarioId)
    {
        var amistades = await _amistadRepository.GetAmistadesByUsuarioIdAsync(usuarioId);
        var amigos = new List<UsuarioDto>();
        
        foreach (var amistad in amistades)
        {
            var amigoEntity = amistad.UsuarioId1 == usuarioId ? amistad.Usuario2 : amistad.Usuario1;
            
            if (amigoEntity != null && amigoEntity.EsActivo)
            {
                amigos.Add(new UsuarioDto
                {
                    Id = amigoEntity.Id,
                    Nombre = amigoEntity.Nombre,
                    Apellido = amigoEntity.Apellido,
                    NombreUsuario = amigoEntity.NombreUsuario,
                    FotoPerfilUrl = amigoEntity.FotoPerfilUrl
                });
            }
        }
        
        return amigos;
    }

    public async Task<int> GetAmigosEnComunCountAsync(int usuarioId1, int usuarioId2)
    {
        var amistadesU1 = await _amistadRepository.GetAmistadesByUsuarioIdAsync(usuarioId1);
        var amistadesU2 = await _amistadRepository.GetAmistadesByUsuarioIdAsync(usuarioId2);

        var amigosU1 = amistadesU1.Select(a => a.UsuarioId1 == usuarioId1 ? a.UsuarioId2 : a.UsuarioId1).ToList();
        var amigosU2 = amistadesU2.Select(a => a.UsuarioId1 == usuarioId2 ? a.UsuarioId2 : a.UsuarioId1).ToList();

        return amigosU1.Intersect(amigosU2).Count();
    }

    public async Task DeleteAmigoAsync(int usuarioId, int amigoId)
    {
        var amistad = await _amistadRepository.GetAmistadEntreUsuariosAsync(usuarioId, amigoId);
        if (amistad == null || amistad.Estado != "Activa")
        {
            throw new ApiException("No se encontró una amistad activa.");
        }

        amistad.Estado = "Eliminada";
        await _amistadRepository.UpdateAsync(amistad);
    }
}
