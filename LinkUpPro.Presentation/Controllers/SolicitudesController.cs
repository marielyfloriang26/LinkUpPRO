using Microsoft.AspNetCore.Mvc;
using LinkUpPro.Presentation.ViewModels.Solicitud;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LinkUpPro.Application.Interfaces.Services;
using System.Security.Claims;
using LinkUpPro.Application.Exceptions;
using System.Linq;

namespace LinkUpPro.Presentation.Controllers
{
    [Authorize(Policy = "CuentaActiva")]
    public class SolicitudesController : Controller
    {
        private readonly ISolicitudAmistadService _solicitudService;

        public SolicitudesController(ISolicitudAmistadService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var pendientesDto = await _solicitudService.GetSolicitudesRecibidasAsync(userId);
            var enviadasDto = await _solicitudService.GetSolicitudesEnviadasAsync(userId);

            var vm = new SolicitudesIndexViewModel
            {
                Pendientes = pendientesDto
                    .Where(s => s.Estado == "En espera de respuesta")
                    .OrderByDescending(s => s.FechaEnvio)
                    .Select(s => new SolicitudAmistadViewModel
                {
                    Id = s.Id,
                    UsuarioId = s.Emisor.Id,
                    Nombre = s.Emisor.Nombre,
                    Apellido = s.Emisor.Apellido,
                    NombreUsuario = s.Emisor.NombreUsuario,
                    FotoPerfilUrl = s.Emisor.FotoPerfilUrl,
                    FechaEnvio = s.FechaEnvio,
                    Estado = s.Estado
                }).ToList(),
                Enviadas = enviadasDto
                    .Where(s => !s.OcultaEnHistorial && s.Estado != "Cancelada")
                    .OrderByDescending(s => s.FechaEnvio)
                    .Select(s => new SolicitudAmistadViewModel
                {
                    Id = s.Id,
                    UsuarioId = s.Receptor.Id,
                    Nombre = s.Receptor.Nombre,
                    Apellido = s.Receptor.Apellido,
                    NombreUsuario = s.Receptor.NombreUsuario,
                    FotoPerfilUrl = s.Receptor.FotoPerfilUrl,
                    FechaEnvio = s.FechaEnvio,
                    FechaRespuesta = s.FechaRespuesta,
                    Estado = s.Estado
                }).ToList()
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Nueva(string query)
        {
            var userId = GetCurrentUserId();
            var usuariosDto = await _solicitudService.BuscarUsuariosParaAgregarAsync(userId, query);

            var usuariosDisponibles = usuariosDto.Select(u => new UsuarioDisponibleViewModel
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                NombreUsuario = u.NombreUsuario,
                FotoPerfilUrl = u.FotoPerfilUrl,
                AmigosEnComun = 0 // Needs calculation injected into BuscarUsuariosParaAgregarAsync or looped here. (Can be ignored safely until View is built or updated later)
            }).ToList();

            ViewBag.SearchQuery = query;
            return View(usuariosDisponibles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar(int id)
        {
            try
            {
                await _solicitudService.SendSolicitudAsync(GetCurrentUserId(), id);
                TempData["SuccessMessage"] = "La solicitud de amistad fue enviada correctamente.";
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud. Inténtelo nuevamente.";
            }
            return RedirectToAction(nameof(Nueva));
        }

        [HttpGet]
        public async Task<IActionResult> Aceptar(int id)
        {
            var userId = GetCurrentUserId();
            var pendientes = await _solicitudService.GetSolicitudesRecibidasAsync(userId);
            var sol = pendientes.FirstOrDefault(s => s.Id == id);
            if (sol == null)
            {
                TempData["ErrorMessage"] = "Esta solicitud ya no se encuentra disponible para ser aceptada.";
                return RedirectToAction(nameof(Index));
            }
            var vm = new LinkUpPro.Presentation.ViewModels.Solicitud.SolicitudAmistadViewModel { Id = sol.Id, NombreUsuario = sol.Emisor.NombreUsuario };
            return View(vm);
        }

        [HttpPost, ActionName("Aceptar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceptarConfirmado(int id)
        {
            try
            {
                await _solicitudService.AcceptSolicitudAsync(id, GetCurrentUserId());
                TempData["SuccessMessage"] = "La solicitud fue aceptada correctamente.";
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud. Inténtelo nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Rechazar(int id)
        {
            var userId = GetCurrentUserId();
            var pendientes = await _solicitudService.GetSolicitudesRecibidasAsync(userId);
            var sol = pendientes.FirstOrDefault(s => s.Id == id);
            if (sol == null) return RedirectToAction(nameof(Index));
            var vm = new LinkUpPro.Presentation.ViewModels.Solicitud.SolicitudAmistadViewModel { Id = sol.Id, NombreUsuario = sol.Emisor.NombreUsuario };
            return View(vm);
        }

        [HttpPost, ActionName("Rechazar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarConfirmado(int id)
        {
            try
            {
                await _solicitudService.RejectSolicitudAsync(id, GetCurrentUserId());
                TempData["SuccessMessage"] = "La solicitud fue rechazada correctamente.";
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud. Inténtelo nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Cancelar(int id)
        {
            var userId = GetCurrentUserId();
            var enviadas = await _solicitudService.GetSolicitudesEnviadasAsync(userId);
            var sol = enviadas.FirstOrDefault(s => s.Id == id);
            if (sol == null) return RedirectToAction(nameof(Index));
            var vm = new LinkUpPro.Presentation.ViewModels.Solicitud.SolicitudAmistadViewModel { Id = sol.Id, NombreUsuario = sol.Receptor.NombreUsuario };
            return View(vm);
        }

        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarConfirmado(int id)
        {
            try
            {
                await _solicitudService.CancelSolicitudAsync(id, GetCurrentUserId());
                TempData["SuccessMessage"] = "La solicitud fue cancelada correctamente.";
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud. Inténtelo nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EliminarHistorial(int id)
        {
            var userId = GetCurrentUserId();
            var enviadas = await _solicitudService.GetSolicitudesEnviadasAsync(userId);
            var sol = enviadas.FirstOrDefault(s => s.Id == id);
            if (sol == null) return RedirectToAction(nameof(Index));
            var vm = new LinkUpPro.Presentation.ViewModels.Solicitud.SolicitudAmistadViewModel { Id = sol.Id, NombreUsuario = sol.Receptor.NombreUsuario };
            return View(vm);
        }

        [HttpPost, ActionName("EliminarHistorial")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarHistorialConfirmado(int id)
        {
            try
            {
                await _solicitudService.RemoveSolicitudFromHistoryAsync(id, GetCurrentUserId());
                TempData["SuccessMessage"] = "La solicitud fue eliminada de su historial.";
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud. Inténtelo nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
