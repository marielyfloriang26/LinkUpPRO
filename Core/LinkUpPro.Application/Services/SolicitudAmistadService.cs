using LinkUpPro.Application.DTOs;
using LinkUpPro.Application.Exceptions;
using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Services;

public class SolicitudAmistadService : ISolicitudAmistadService
{
    private readonly ISolicitudAmistadRepository _solicitudRepository;
    private readonly IAmistadRepository _amistadRepository;
    private readonly IRepositoryAsync<Usuario> _usuarioRepository;

    public SolicitudAmistadService(
        ISolicitudAmistadRepository solicitudRepository,
        IAmistadRepository amistadRepository,
        IRepositoryAsync<Usuario> usuarioRepository)
    {
        _solicitudRepository = solicitudRepository;
        _amistadRepository = amistadRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task SendSolicitudAsync(int emisorId, int receptorId)
    {
        if (emisorId == receptorId) throw new ApiException("No puedes enviarte una solicitud a ti mismo.");

        var emisor = await _usuarioRepository.GetByIdAsync(emisorId);
        var receptor = await _usuarioRepository.GetByIdAsync(receptorId);

        if (emisor == null || !emisor.EsActivo) throw new ApiException("El usuario emisor no existe o no está activo.");
        if (receptor == null || !receptor.EsActivo) throw new ApiException("El usuario receptor no existe o no está activo.");

        var amistadExistente = await _amistadRepository.GetAmistadEntreUsuariosAsync(emisorId, receptorId);
        if (amistadExistente != null && amistadExistente.Estado == "Activa")
        {
            throw new ApiException("Ya existe una amistad activa con este usuario.");
        }

        var solicitudPendiente = await _solicitudRepository.GetSolicitudPendienteAsync(emisorId, receptorId);
        if (solicitudPendiente != null)
        {
            throw new ApiException("Ya hay una solicitud de amistad pendiente enviada a este usuario.");
        }
        
        var solicitudPendienteInversa = await _solicitudRepository.GetSolicitudPendienteAsync(receptorId, emisorId);
        if (solicitudPendienteInversa != null)
        {
            throw new ApiException("Este usuario ya te ha enviado una solicitud de amistad.");
        }

        var nuevaSolicitud = new SolicitudAmistad
        {
            EmisorId = emisorId,
            ReceptorId = receptorId,
            Estado = "Pendiente",
            FechaEnvio = DateTime.UtcNow
        };

        await _solicitudRepository.AddAsync(nuevaSolicitud);
    }

    public async Task AcceptSolicitudAsync(int solicitudId, int receptorId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null || solicitud.ReceptorId != receptorId)
        {
            throw new ApiException("Solicitud no encontrada o no autorizada.");
        }

        if (solicitud.Estado != "Pendiente")
        {
            throw new ApiException("La solicitud no está pendiente (Estado: " + solicitud.Estado + ").");
        }

        solicitud.Estado = "Aceptada";
        await _solicitudRepository.UpdateAsync(solicitud);

        var amistadInactiva = await _amistadRepository.GetAmistadEntreUsuariosAsync(solicitud.EmisorId, solicitud.ReceptorId);
        if (amistadInactiva != null)
        {
            amistadInactiva.Estado = "Activa";
            await _amistadRepository.UpdateAsync(amistadInactiva);
        }
        else
        {
            var nuevaAmistad = new Amistad
            {
                UsuarioId1 = solicitud.EmisorId,
                UsuarioId2 = solicitud.ReceptorId,
                FechaAmistad = DateTime.UtcNow,
                Estado = "Activa"
            };
            await _amistadRepository.AddAsync(nuevaAmistad);
        }
    }

    public async Task RejectSolicitudAsync(int solicitudId, int receptorId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null || solicitud.ReceptorId != receptorId)
        {
            throw new ApiException("Solicitud no encontrada o no autorizada.");
        }
        if (solicitud.Estado != "Pendiente") throw new ApiException("La solicitud no está pendiente.");

        solicitud.Estado = "Rechazada";
        await _solicitudRepository.UpdateAsync(solicitud);
    }

    public async Task CancelSolicitudAsync(int solicitudId, int emisorId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null || solicitud.EmisorId != emisorId)
        {
            throw new ApiException("Solicitud no encontrada o no autorizada.");
        }
        if (solicitud.Estado != "Pendiente") throw new ApiException("La solicitud no está pendiente.");

        solicitud.Estado = "Cancelada";
        await _solicitudRepository.UpdateAsync(solicitud);
    }

    public async Task RemoveSolicitudFromHistoryAsync(int solicitudId, int usuarioId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null) throw new ApiException("Solicitud no encontrada.");
        
        if (solicitud.EmisorId != usuarioId && solicitud.ReceptorId != usuarioId)
        {
            throw new ApiException("No autorizado.");
        }

        if (solicitud.Estado == "Aceptada" || solicitud.Estado == "Rechazada" || solicitud.Estado == "Cancelada")
        {
            await _solicitudRepository.DeleteAsync(solicitud);
        }
        else
        {
            throw new ApiException("Solo se pueden eliminar del historial las solicitudes finalizadas (Aceptada, Rechazada, Cancelada).");
        }
    }

    public async Task<IReadOnlyList<SolicitudAmistadDto>> GetSolicitudesRecibidasAsync(int receptorId)
    {
        var solicitudes = await _solicitudRepository.GetSolicitudesRecibidasAsync(receptorId);
        return solicitudes.Select(s => new SolicitudAmistadDto
        {
            Id = s.Id,
            EmisorId = s.EmisorId,
            ReceptorId = s.ReceptorId,
            Estado = s.Estado,
            FechaEnvio = s.FechaEnvio,
            Emisor = new UsuarioDto
            {
                Id = s.Emisor.Id,
                Nombre = s.Emisor.Nombre,
                Apellido = s.Emisor.Apellido,
                NombreUsuario = s.Emisor.NombreUsuario,
                FotoPerfilUrl = s.Emisor.FotoPerfilUrl
            }
        }).ToList();
    }

    public async Task<IReadOnlyList<SolicitudAmistadDto>> GetSolicitudesEnviadasAsync(int emisorId)
    {
        var solicitudes = await _solicitudRepository.GetSolicitudesEnviadasAsync(emisorId);
        return solicitudes.Select(s => new SolicitudAmistadDto
        {
            Id = s.Id,
            EmisorId = s.EmisorId,
            ReceptorId = s.ReceptorId,
            Estado = s.Estado,
            FechaEnvio = s.FechaEnvio,
            Receptor = new UsuarioDto
            {
                Id = s.Receptor.Id,
                Nombre = s.Receptor.Nombre,
                Apellido = s.Receptor.Apellido,
                NombreUsuario = s.Receptor.NombreUsuario,
                FotoPerfilUrl = s.Receptor.FotoPerfilUrl
            }
        }).ToList();
    }

    public async Task<IReadOnlyList<UsuarioDto>> BuscarUsuariosParaAgregarAsync(int currentUserId, string searchString)
    {
        var todosLosUsuarios = await _usuarioRepository.GetAllAsync();
        
        var amistadesActuales = await _amistadRepository.GetAmistadesByUsuarioIdAsync(currentUserId);
        var idsAmigos = amistadesActuales.Select(a => a.UsuarioId1 == currentUserId ? a.UsuarioId2 : a.UsuarioId1).ToHashSet();

        var solicitudesEnviadas = await _solicitudRepository.GetSolicitudesEnviadasAsync(currentUserId);
        var idsEnviados = solicitudesEnviadas.Select(s => s.ReceptorId).ToHashSet();

        var solicitudesRecibidas = await _solicitudRepository.GetSolicitudesRecibidasAsync(currentUserId);
        var idsRecibidos = solicitudesRecibidas.Select(s => s.EmisorId).ToHashSet();

        var lowerSearch = string.IsNullOrWhiteSpace(searchString) ? "" : searchString.ToLower();

        var result = todosLosUsuarios.Where(u => 
            u.Id != currentUserId && 
            u.EsActivo &&
            !idsAmigos.Contains(u.Id) &&
            !idsEnviados.Contains(u.Id) &&
            !idsRecibidos.Contains(u.Id) &&
            (string.IsNullOrWhiteSpace(lowerSearch) || 
             u.Nombre.ToLower().Contains(lowerSearch) || 
             u.Apellido.ToLower().Contains(lowerSearch) || 
             u.NombreUsuario.ToLower().Contains(lowerSearch))
        ).Select(u => new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            NombreUsuario = u.NombreUsuario,
            FotoPerfilUrl = u.FotoPerfilUrl
        }).ToList();

        return result;
    }
}
