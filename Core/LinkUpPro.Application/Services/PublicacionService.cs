using AutoMapper;
using LinkUpPro.Application.DTOs.Publicacion;
using LinkUpPro.Application.DTOs.PublicacionDTO;
using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Domain.Entities;

namespace LinkUpPro.Application.Services;

public class PublicacionService : IPublicacionService
{
    private readonly IPublicacionRepository _publicacionRepository;
    private readonly IMapper _mapper;

    public PublicacionService(IPublicacionRepository publicacionRepository, IMapper mapper)
    {
        _publicacionRepository = publicacionRepository;
        _mapper = mapper;
    }

    
    public async Task<List<PublicacionDto>> ObtenerTodasAsync()
    {
        var publicacionesEntidad = await _publicacionRepository.GetTodasConDetallesAsync();

        // Mapea la lista de entidades a la lista de publicaciondto usando AutoMapper
        var listaDtos = _mapper.Map<List<PublicacionDto>>(publicacionesEntidad);

        return listaDtos;
    }

   
    public async Task CrearAsync(CrearPublicacionDto dto)
    {
        // Mapea el dto de entrada a la entidad de dominioa
        var nuevaPublicacion = _mapper.Map<Publicacion>(dto);
        
        
        nuevaPublicacion.FechaCreacion = DateTime.UtcNow;

        await _publicacionRepository.AddAsync(nuevaPublicacion);
    }

   
    public async Task EditarAsync(ModificarPublicacionDto dto)
    {
        // Busca primero el registro original de la bd
        var publicacionExistente = await _publicacionRepository.GetByIdAsync(dto.Id);

        if (publicacionExistente != null)
        {
            
            // Pasa los cambios del dto a la entidad recuperada
            publicacionExistente.ContenidoTexto = dto.ContenidoTexto;
            publicacionExistente.ImagenUrl = dto.ImagenUrl;
            publicacionExistente.YouTubeVideoUrl = dto.YouTubeVideoUrl;
            publicacionExistente.Privacidad = dto.Privacidad;

            await _publicacionRepository.UpdateAsync(publicacionExistente);
        }
    }

    
    public async Task EliminarAsync(int id)
    {
        var publicacion = await _publicacionRepository.GetByIdAsync(id);
        if (publicacion != null)
        {
            await _publicacionRepository.DeleteAsync(publicacion);
        }
    }
}