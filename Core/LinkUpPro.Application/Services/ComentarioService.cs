using System;
using System.Threading.Tasks;
using AutoMapper;
using LinkUpPro.Application.DTOs.ComentarioDTO;
using LinkUpPro.Application.Interface.Services;
using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Domain.Entities;

namespace LinkUpPro.Application.Services;

public class ComentarioService : IComentarioService
{
    private readonly IRepositoryAsync<Comentario> _comentarioRepository;
    private readonly IPublicacionRepository _publicacionRepository;
    private readonly IRepositoryAsync<Usuario> _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly INotificacionService _notificacionService;

    public ComentarioService(
        IRepositoryAsync<Comentario> comentarioRepository,
        IPublicacionRepository publicacionRepository,
        IRepositoryAsync<Usuario> usuarioRepository,
        IMapper mapper, INotificacionService notificacionService)
    {
        _comentarioRepository = comentarioRepository;
        _publicacionRepository = publicacionRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _notificacionService = notificacionService;
    }

    public async Task CrearAsync(CrearComentarioDto dto)
    {
        // Contenido requerido y no compuesto por puros espacios
        if (string.IsNullOrWhiteSpace(dto.Contenido))
        {
            throw new Exception("El contenido del comentario es requerido.");
        }

        // Maximo de 500 caracteres
        if (dto.Contenido.Length > 500)
        {
            throw new Exception("El comentario no debe exceder los 500 caracteres.");
        }

        // La publicacion debe existir y encontrarse activa (no eliminada)
        var publicacion = await _publicacionRepository.GetByIdAsync(dto.PublicacionId);
        if (publicacion == null || publicacion.Estado == "Eliminada")
        {
            throw new Exception("La publicación no existe o no se encuentra activa.");
        }

        // La publicacion debe permitir comentarios
        if (!publicacion.PermiteComentarios)
        {
            throw new Exception("Los comentarios están desactivados para esta publicación.");
        }

        // El usuario debe encontrarse autenticado y activo
        var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId);
        if (usuario == null || !usuario.EmailConfirmed)
        {
            throw new Exception("El usuario no se encuentra activo.");
        }

        // Si todo es válido, guardamos
        var nuevoComentario = _mapper.Map<Comentario>(dto);
        nuevoComentario.FechaCreacion = DateTime.UtcNow;

        await _comentarioRepository.AddAsync(nuevoComentario);

        // Dispara Notificacion
        if (nuevoComentario.ComentarioPadreId.HasValue)
        {
            // Es una respuesta a otro comentario
            var comentarioPadre = await _comentarioRepository.GetByIdAsync(nuevoComentario.ComentarioPadreId.Value);
            if (comentarioPadre != null && comentarioPadre.UsuarioId != nuevoComentario.UsuarioId)
            {
                var msg = $"{usuario.UserName} respondió tu comentario.";
                await _notificacionService.CrearNotificacionAsync(
                    comentarioPadre.UsuarioId, 
                    nuevoComentario.UsuarioId, 
                    nuevoComentario.PublicacionId, 
                    "Respuesta", 
                    msg);
            }
        }
        else
        {
            // Es un comentario principal en una publi
            if (publicacion.UsuarioId != nuevoComentario.UsuarioId)
            {
                var msg = $"{usuario.UserName} comentó tu publicación.";
                await _notificacionService.CrearNotificacionAsync(
                    publicacion.UsuarioId, 
                    nuevoComentario.UsuarioId, 
                    nuevoComentario.PublicacionId, 
                    "Comentario", 
                    msg);
            }
        }
    }

    public async Task EditarAsync(int id, string contenido, int usuarioId)
{
    var comentario = await _comentarioRepository.GetByIdAsync(id);
    if (comentario == null) throw new Exception("El comentario no existe.");
    
    // Valida que sea el autor
    if (comentario.UsuarioId != usuarioId)
    {
        throw new Exception("No posee permisos para editar este contenido.");
    }

    // Valida contenido
    if (string.IsNullOrWhiteSpace(contenido))
    {
        throw new Exception("El contenido del comentario es requerido.");
    }
    if (contenido.Length > 500)
    {
        throw new Exception("El comentario no debe exceder los 500 caracteres.");
    }

    comentario.Contenido = contenido;
    comentario.FechaModificacion = DateTime.UtcNow;

    await _comentarioRepository.UpdateAsync(comentario);
}

public async Task EliminarAsync(int id, int usuarioId)
{
    var comentario = await _comentarioRepository.GetByIdAsync(id);
    if (comentario == null) throw new Exception("El comentario no existe.");

    if (comentario.UsuarioId != usuarioId)
    {
        throw new Exception("No posee permisos para eliminar este contenido.");
    }

    // Comprueba si tiene respuestas anidadas activas
    var todosLosComentarios = await _comentarioRepository.GetAllAsync();
    var tieneRespuestas = todosLosComentarios.Any(c => c.ComentarioPadreId == id && c.Contenido != "Este comentario fue eliminado.");

    if (tieneRespuestas)
    {
        // Si tiene respuestas, conserva su posicion pero reemplaza el contenido
        comentario.Contenido = "Este comentario fue eliminado.";
        await _comentarioRepository.UpdateAsync(comentario);
    }
    else
    {
        // Si no tiene respuestas, lo borra fisicamente
        await _comentarioRepository.DeleteAsync(comentario);
    }
}

    public async Task<ComentarioDto?> ObtenerPorIdAsync(int id)
    {
        var comentario = await _comentarioRepository.GetByIdAsync(id);
        if (comentario == null) return null;

        return _mapper.Map<ComentarioDto>(comentario);
    }
}