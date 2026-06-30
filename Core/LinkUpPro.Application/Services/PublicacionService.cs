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
    private readonly IAmistadRepository _amistadRepository;
    private readonly IMapper _mapper;

    public PublicacionService(IPublicacionRepository publicacionRepository, IAmistadRepository amistadRepository, IMapper mapper)
    {
        _publicacionRepository = publicacionRepository;
        _amistadRepository = amistadRepository;
        _mapper = mapper;
    }

    
    public async Task<List<PublicacionDto>> ObtenerTodasAsync(int currentUserId, string? textoBusqueda = null, 
        string? tipoContenido = null, 
        string? estadoEdicion = null, 
        DateTime? fechaDesde = null, 
        DateTime? fechaHasta = null)
    {
        var publicacionesEntidad = await _publicacionRepository.GetTodasConDetallesAsync();

        var query = publicacionesEntidad.Where(p => p.UsuarioId == currentUserId && p.Estado == "Activa");

        // La busqueda por texto no distingue mayusculas/minusculas e ignora espacios al inicio y final
        if (!string.IsNullOrWhiteSpace(textoBusqueda))
        {
            var textoLimpio = textoBusqueda.Trim().ToLower();
            query = query.Where(p => p.ContenidoTexto != null && p.ContenidoTexto.ToLower().Contains(textoLimpio));
        }

        // Filtro por Tipo de Contenido 
        if (!string.IsNullOrEmpty(tipoContenido) && tipoContenido != "Todos")
        {
            if (tipoContenido == "Imagen")
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.ImagenUrl));
            }
            else if (tipoContenido == "Video")
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.YouTubeVideoUrl));
            }
        }

        // Filtro por Estado de edicion 
        if (!string.IsNullOrEmpty(estadoEdicion) && estadoEdicion != "Todas")
        {
            if (estadoEdicion == "Editadas")
            {
                query = query.Where(p => p.FechaModificacion.HasValue);
            }
            else if (estadoEdicion == "NoEditadas")
            {
                query = query.Where(p => !p.FechaModificacion.HasValue);
            }
            
        }

        // Filtros de fechas, fecha desde incluye las publicaciones realizadas a partir de esa fecha
        if (fechaDesde.HasValue)
        {
            query = query.Where(p => p.FechaCreacion.Date >= fechaDesde.Value.Date);
        }

        // Fecha hasta debe incluir las publicaciones realizadas hasta el final de esa fecha
        if (fechaHasta.HasValue)
        {
            query = query.Where(p => p.FechaCreacion.Date <= fechaHasta.Value.Date);
        }

        // Las publicaciones deben organizarse desde la mas reciente hasta la mas antigua utilizando fecha y hora
        var listaOrdenada = query.OrderByDescending(p => p.FechaCreacion).ToList();

        // Mapeo final al dto usando AutoMapper
        return _mapper.Map<List<PublicacionDto>>(listaOrdenada);
    }

    public async Task<List<PublicacionDto>> ObtenerPublicacionesAmigosAsync(
        int currentUserId, 
        string? textoBusqueda = null, 
        int? amigoId = null,
        string? tipoContenido = null, 
        string? estadoEdicion = null, 
        DateTime? fechaDesde = null, 
        DateTime? fechaHasta = null)
    {
        // 1. Obtener amigos activos del usuario actual
        var amistades = await _amistadRepository.GetAmistadesByUsuarioIdAsync(currentUserId);
        
        var amigosIds = amistades.Select(a => a.UsuarioId1 == currentUserId ? a.Usuario2 : a.Usuario1)
                                 .Where(u => u.EmailConfirmed)
                                 .Select(u => u.Id)
                                 .ToList();

        // 2. Obtener publicaciones y filtrar
        var publicacionesEntidad = await _publicacionRepository.GetTodasConDetallesAsync();

        var query = publicacionesEntidad.Where(p => 
            amigosIds.Contains(p.UsuarioId) && 
            p.Privacidad == "SoloAmigos" && 
            p.Estado == "Activa");

        // 3. Aplicar filtros adicionales
        if (amigoId.HasValue)
        {
            query = query.Where(p => p.UsuarioId == amigoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(textoBusqueda))
        {
            var textoLimpio = textoBusqueda.Trim().ToLower();
            query = query.Where(p => p.ContenidoTexto != null && p.ContenidoTexto.ToLower().Contains(textoLimpio));
        }

        if (!string.IsNullOrEmpty(tipoContenido) && tipoContenido != "Todos")
        {
            if (tipoContenido == "Imagen")
                query = query.Where(p => !string.IsNullOrEmpty(p.ImagenUrl));
            else if (tipoContenido == "Video")
                query = query.Where(p => !string.IsNullOrEmpty(p.YouTubeVideoUrl));
        }

        if (!string.IsNullOrEmpty(estadoEdicion) && estadoEdicion != "Todas")
        {
            if (estadoEdicion == "Editadas")
                query = query.Where(p => p.FechaModificacion.HasValue);
            else if (estadoEdicion == "NoEditadas")
                query = query.Where(p => !p.FechaModificacion.HasValue);
        }

        if (fechaDesde.HasValue)
            query = query.Where(p => p.FechaCreacion.Date >= fechaDesde.Value.Date);

        if (fechaHasta.HasValue)
            query = query.Where(p => p.FechaCreacion.Date <= fechaHasta.Value.Date);

        // 4. Ordenar y retornar
        var listaOrdenada = query.OrderByDescending(p => p.FechaCreacion).ToList();

        return _mapper.Map<List<PublicacionDto>>(listaOrdenada);
    }

   
    public async Task CrearAsync(CrearPublicacionDto dto)
    {
        // El contenido no debe estar vacio ni ser puros espacios
        if (string.IsNullOrWhiteSpace(dto.ContenidoTexto))
        {
            throw new Exception("Debe ingresar el contenido de la publicación.");
        }

        if (dto.ContenidoTexto.Length > 1000)
        {
            throw new Exception("La publicación no puede exceder los 1000 caracteres.");
        }

        // Extraccion y conversion a Embed
        if (!string.IsNullOrEmpty(dto.YouTubeVideoUrl))
        {
            var videoId = ExtraerYouTubeId(dto.YouTubeVideoUrl);
            if (string.IsNullOrEmpty(videoId))
            {
                throw new Exception("Debe ingresar un enlace válido de YouTube.");
            }
            
            // Convierte la url normal a url embebida 
            dto.YouTubeVideoUrl = $"https://www.youtube.com/embed/{videoId}";
            dto.ImagenUrl = null; // No se permiten ambos juntos
        }

        // Mapea el dto de entrada a la entidad de dominio
        var nuevaPublicacion = _mapper.Map<Publicacion>(dto);
        nuevaPublicacion.FechaCreacion = DateTime.UtcNow;

        await _publicacionRepository.AddAsync(nuevaPublicacion);
    }

    private string? ExtraerYouTubeId(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;

        try
        {
            var uri = new Uri(url);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            if (query.TryGetValue("v", out var videoId))
            {
                return videoId;
            }
            if (uri.Host == "youtu.be")
            {
                return uri.AbsolutePath.TrimStart('/');
            }
            if (uri.AbsolutePath.Contains("/shorts/"))
            {
                return uri.AbsolutePath.Split("/shorts/").Last().Split('?').First();
            }
        }
        catch
        {
            return null;
        }
        return null;
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
            publicacionExistente.PermiteComentarios = dto.PermiteComentarios;
            publicacionExistente.FechaModificacion = DateTime.UtcNow;

            await _publicacionRepository.UpdateAsync(publicacionExistente);
        }
    }

    
    public async Task EliminarAsync(int id)
    {
        var publicacion = await _publicacionRepository.GetByIdAsync(id);
        if (publicacion != null)
        {
            publicacion.Estado = "Eliminada";
            await _publicacionRepository.UpdateAsync(publicacion);
        }
    }
}
