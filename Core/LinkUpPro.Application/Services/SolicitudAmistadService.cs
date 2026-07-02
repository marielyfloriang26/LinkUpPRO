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

        if (emisor == null || !emisor.EmailConfirmed) throw new ApiException("No se puede enviar la solicitud porque uno de los usuarios se encuentra inactivo.");
        if (receptor == null || !receptor.EmailConfirmed) throw new ApiException("No se puede enviar la solicitud porque uno de los usuarios se encuentra inactivo.");

        var amistadExistente = await _amistadRepository.GetAmistadEntreUsuariosAsync(emisorId, receptorId);
        if (amistadExistente != null && amistadExistente.Estado == "Activa")
        {
            throw new ApiException("Este usuario ya forma parte de su lista de amigos.");
        }

        var solicitudPendiente = await _solicitudRepository.GetSolicitudPendienteAsync(emisorId, receptorId);
        if (solicitudPendiente != null)
        {
            throw new ApiException("Ya envió una solicitud de amistad a este usuario y se encuentra pendiente de respuesta.");
        }
        
        var solicitudPendienteInversa = await _solicitudRepository.GetSolicitudPendienteAsync(receptorId, emisorId);
        if (solicitudPendienteInversa != null)
        {
            throw new ApiException("Este usuario ya le envió una solicitud de amistad. Debe aceptarla o rechazarla desde sus solicitudes pendientes.");
        }

        var nuevaSolicitud = new SolicitudAmistad
        {
            EmisorId = emisorId,
            ReceptorId = receptorId,
            Estado = "En espera de respuesta",
            FechaEnvio = DateTime.UtcNow,
            OcultaParaEmisor = false
        };

        await _solicitudRepository.AddAsync(nuevaSolicitud);
    }

    public async Task AcceptSolicitudAsync(int solicitudId, int receptorId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null || solicitud.ReceptorId != receptorId)
        {
            throw new ApiException("Esta solicitud ya no se encuentra disponible para ser aceptada.");
        }

        if (solicitud.Estado != "En espera de respuesta")
        {
            throw new ApiException("Esta solicitud ya no se encuentra disponible para ser aceptada.");
        }

        if (solicitud.EmisorId == solicitud.ReceptorId)
        {
            throw new ApiException("Esta solicitud ya no se encuentra disponible para ser aceptada.");
        }

        var amistadExistente = await _amistadRepository.GetAmistadEntreUsuariosAsync(solicitud.EmisorId, solicitud.ReceptorId);
        if (amistadExistente != null && amistadExistente.Estado == "Activa")
        {
            throw new ApiException("Esta solicitud ya no se encuentra disponible para ser aceptada.");
        }

        var emisor = await _usuarioRepository.GetByIdAsync(solicitud.EmisorId);
        var receptor = await _usuarioRepository.GetByIdAsync(solicitud.ReceptorId);
        if (emisor == null || !emisor.EmailConfirmed || receptor == null || !receptor.EmailConfirmed)
        {
            throw new ApiException("No se puede aceptar la solicitud porque uno de los usuarios se encuentra inactivo.");
        }

        try
        {
            await _solicitudRepository.BeginTransactionAsync();

            solicitud.Estado = "Aceptada";
            solicitud.FechaRespuesta = DateTime.UtcNow;
            await _solicitudRepository.UpdateAsync(solicitud);

            if (amistadExistente != null)
            {
                amistadExistente.Estado = "Activa";
                await _amistadRepository.UpdateAsync(amistadExistente);
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

            await _solicitudRepository.CommitTransactionAsync();
        }
        catch
        {
            await _solicitudRepository.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RejectSolicitudAsync(int solicitudId, int receptorId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null || solicitud.ReceptorId != receptorId)
        {
            throw new ApiException("No posee permisos para realizar esta acción sobre la solicitud.");
        }
        if (solicitud.Estado != "En espera de respuesta") throw new ApiException("Esta solicitud ya no se encuentra disponible para ser rechazada.");

        var emisor = await _usuarioRepository.GetByIdAsync(solicitud.EmisorId);
        var receptor = await _usuarioRepository.GetByIdAsync(solicitud.ReceptorId);
        if (emisor == null || !emisor.EmailConfirmed || receptor == null || !receptor.EmailConfirmed)
        {
            throw new ApiException("No se puede rechazar la solicitud porque uno de los usuarios se encuentra inactivo.");
        }

        solicitud.Estado = "Rechazada";
        solicitud.FechaRespuesta = DateTime.UtcNow;
        await _solicitudRepository.UpdateAsync(solicitud);
    }

    public async Task CancelSolicitudAsync(int solicitudId, int emisorId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null || solicitud.EmisorId != emisorId)
        {
            throw new ApiException("No posee permisos para realizar esta acción sobre la solicitud.");
        }
        if (solicitud.Estado != "En espera de respuesta") throw new ApiException("Esta solicitud ya no se encuentra disponible para ser cancelada.");

        var emisor = await _usuarioRepository.GetByIdAsync(solicitud.EmisorId);
        var receptor = await _usuarioRepository.GetByIdAsync(solicitud.ReceptorId);
        if (emisor == null || !emisor.EmailConfirmed || receptor == null || !receptor.EmailConfirmed)
        {
            throw new ApiException("No se puede cancelar la solicitud porque uno de los usuarios se encuentra inactivo.");
        }

        solicitud.Estado = "Cancelada";
        solicitud.FechaRespuesta = DateTime.UtcNow; 
        await _solicitudRepository.UpdateAsync(solicitud);
    }

    public async Task RemoveSolicitudFromHistoryAsync(int solicitudId, int usuarioId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null || solicitud.EmisorId != usuarioId)
        {
            throw new ApiException("No posee permisos para realizar esta acción sobre la solicitud.");
        }

        var emisor = await _usuarioRepository.GetByIdAsync(solicitud.EmisorId);
        var receptor = await _usuarioRepository.GetByIdAsync(solicitud.ReceptorId);
        if (emisor == null || !emisor.EmailConfirmed || receptor == null || !receptor.EmailConfirmed)
        {
            throw new ApiException("No se puede eliminar la solicitud porque uno de los usuarios se encuentra inactivo.");
        }

        if (solicitud.Estado == "Aceptada" || solicitud.Estado == "Rechazada")
        {
            if (solicitud.OcultaParaEmisor) throw new ApiException("La solicitud ya ha sido ocultada anteriormente.");
            
            solicitud.OcultaParaEmisor = true;
            await _solicitudRepository.UpdateAsync(solicitud);
        }
        else
        {
            throw new ApiException("Solo se pueden eliminar del historial las solicitudes en estado Aceptada o Rechazada.");
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
                Id = s.Emisor!.Id,
                Nombre = s.Emisor.Nombre,
                Apellido = s.Emisor.Apellido,
                NombreUsuario = s.Emisor.UserName ?? "",
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
            FechaRespuesta = s.FechaRespuesta,
            OcultaEnHistorial = s.OcultaParaEmisor,
            Receptor = new UsuarioDto
            {
                Id = s.Receptor!.Id,
                Nombre = s.Receptor.Nombre,
                Apellido = s.Receptor.Apellido,
                NombreUsuario = s.Receptor.UserName ?? "",
                FotoPerfilUrl = s.Receptor.FotoPerfilUrl
            }
        }).ToList();
    }

    public async Task<IReadOnlyList<UsuarioDto>> BuscarUsuariosParaAgregarAsync(int currentUserId, string searchString)
    {
        var todosLosUsuarios = await _usuarioRepository.GetAllAsync();
        
        var amistadesActuales = await _amistadRepository.GetAmistadesByUsuarioIdAsync(currentUserId);
        var idsAmigos = amistadesActuales.Where(a => a.Estado == "Activa").Select(a => a.UsuarioId1 == currentUserId ? a.UsuarioId2 : a.UsuarioId1).ToHashSet();

        // Obtener solo las pendientes para excluir
        var pendientesEnviadas = await _solicitudRepository.GetSolicitudesEnviadasAsync(currentUserId);
        var idsEnviados = pendientesEnviadas.Where(s => s.Estado == "En espera de respuesta").Select(s => s.ReceptorId).ToHashSet();

        var pendientesRecibidas = await _solicitudRepository.GetSolicitudesRecibidasAsync(currentUserId);
        var idsRecibidos = pendientesRecibidas.Where(s => s.Estado == "En espera de respuesta").Select(s => s.EmisorId).ToHashSet();

        var lowerSearch = string.IsNullOrWhiteSpace(searchString) ? "" : searchString.Trim().ToLower();

        var result = todosLosUsuarios.Where(u => 
            u.Id != currentUserId && 
            u.EmailConfirmed &&
            !idsAmigos.Contains(u.Id) &&
            !idsEnviados.Contains(u.Id) &&
            !idsRecibidos.Contains(u.Id) &&
            (string.IsNullOrWhiteSpace(lowerSearch) || 
             u.Nombre.ToLower().Contains(lowerSearch) || 
             u.Apellido.ToLower().Contains(lowerSearch) || 
             (u.UserName != null && u.UserName.ToLower().Contains(lowerSearch)))
        ).Select(u => new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            NombreUsuario = u.UserName ?? "",
            FotoPerfilUrl = u.FotoPerfilUrl
        }).ToList();

        return result;
    }
}
