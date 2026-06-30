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

    public ComentarioService(
        IRepositoryAsync<Comentario> comentarioRepository,
        IPublicacionRepository publicacionRepository,
        IRepositoryAsync<Usuario> usuarioRepository,
        IMapper mapper)
    {
        _comentarioRepository = comentarioRepository;
        _publicacionRepository = publicacionRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
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
        if (publicacion == null || publicacion.EstaEliminada)
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
        if (usuario == null || !usuario.EsActivo)
        {
            throw new Exception("El usuario no se encuentra activo.");
        }

        // Si todo es válido, guardamos
        var nuevoComentario = _mapper.Map<Comentario>(dto);
        nuevoComentario.FechaCreacion = DateTime.UtcNow;

        await _comentarioRepository.AddAsync(nuevoComentario);
    }
}