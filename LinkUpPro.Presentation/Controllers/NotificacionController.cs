using AutoMapper;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.ViewModels.Notificacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkUpPro.Presentation.Controllers;

[Authorize(Policy = "CuentaActiva")]
public class NotificacionController : Controller
{
    private readonly INotificacionService _notificacionService;
    private readonly IPublicacionRepository _publicacionRepository;
    private readonly IMapper _mapper;

    public NotificacionController(
        INotificacionService notificacionService,
        IPublicacionRepository publicacionRepository,
        IMapper mapper)
    {
        _notificacionService = notificacionService;
        _publicacionRepository = publicacionRepository;
        _mapper = mapper;
    }

    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int usuarioId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var dtos = await _notificacionService.ObtenerTodasPorUsuarioAsync(usuarioId);
        var vms = _mapper.Map<List<NotificacionViewModel>>(dtos);

        return View(vms);
    }

    
    [HttpGet]
    public async Task<IActionResult> IrAlContenido(int id)
    {
        int usuarioId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var dtos = await _notificacionService.ObtenerTodasPorUsuarioAsync(usuarioId);
        var noti = dtos.Find(n => n.Id == id);

        if (noti == null) return NotFound();

        // Marcar como leida de forma auto al entrar
        await _notificacionService.MarcarComoLeidaAsync(id, usuarioId);

        // Si la publi no tiene id asignado o fue borrada, muestra el error de requerimiento
        if (!noti.PublicacionId.HasValue)
        {
            TempData["MensajeError"] = "El contenido relacionado con esta notificación ya no se encuentra disponible.";
            return RedirectToAction("Index");
        }

        var publicacion = await _publicacionRepository.GetByIdAsync(noti.PublicacionId.Value);
        if (publicacion == null || publicacion.Estado == "Eliminada")
        {
            TempData["MensajeError"] = "El contenido relacionado con esta notificación ya no se encuentra disponible.";
            return RedirectToAction("Index");
        }

        // Redirige a la vista de comentarios de la publi correspondiente
        return RedirectToAction("Comentarios", "Publicacion", new { id = noti.PublicacionId.Value });
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarComoLeida(int id)
    {
        int usuarioId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        await _notificacionService.MarcarComoLeidaAsync(id, usuarioId);
        return RedirectToAction("Index");
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarTodasComoLeidas()
    {
        int usuarioId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        await _notificacionService.MarcarTodasComoLeidasAsync(usuarioId);
        return RedirectToAction("Index");
    }
}