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
        private readonly IAmigoService _amigoService;

        public SolicitudesController(ISolicitudAmistadService solicitudService, IAmigoService amigoService)
        {
            _solicitudService = solicitudService;
            _amigoService = amigoService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                var pendientesDto = await _solicitudService.GetSolicitudesRecibidasAsync(userId);
                var enviadasDto = await _solicitudService.GetSolicitudesEnviadasAsync(userId);

                var vm = new SolicitudesIndexViewModel
                {
                    Pendientes = new List<SolicitudAmistadViewModel>(),
                    Enviadas = new List<SolicitudAmistadViewModel>()
                };

                foreach (var s in pendientesDto.Where(s => s.Estado == "En espera de respuesta").OrderByDescending(s => s.FechaEnvio))
                {
                    vm.Pendientes.Add(new SolicitudAmistadViewModel
                    {
                        Id = s.Id,
                        UsuarioId = s.Emisor.Id,
                        Nombre = s.Emisor.Nombre,
                        Apellido = s.Emisor.Apellido,
                        NombreUsuario = s.Emisor.NombreUsuario,
                        FotoPerfilUrl = s.Emisor.FotoPerfilUrl,
                        FechaEnvio = s.FechaEnvio,
                        Estado = s.Estado,
                        AmigosEnComun = await _amigoService.GetAmigosEnComunCountAsync(userId, s.Emisor.Id)
                    });
                }

                foreach (var s in enviadasDto.Where(s => !s.OcultaEnHistorial && s.Estado != "Cancelada").OrderByDescending(s => s.FechaEnvio))
                {
                    vm.Enviadas.Add(new SolicitudAmistadViewModel
                    {
                        Id = s.Id,
                        UsuarioId = s.Receptor.Id,
                        Nombre = s.Receptor.Nombre,
                        Apellido = s.Receptor.Apellido,
                        NombreUsuario = s.Receptor.NombreUsuario,
                        FotoPerfilUrl = s.Receptor.FotoPerfilUrl,
                        FechaEnvio = s.FechaEnvio,
                        FechaRespuesta = s.FechaRespuesta,
                        Estado = s.Estado,
                        AmigosEnComun = await _amigoService.GetAmigosEnComunCountAsync(userId, s.Receptor.Id)
                    });
                }
                return View(vm);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al cargar las solicitudes.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Nueva(string query)
        {
            try
            {
                var userId = GetCurrentUserId();
                var usuariosDto = await _solicitudService.BuscarUsuariosParaAgregarAsync(userId, query);

                var usuariosDisponibles = new List<UsuarioDisponibleViewModel>();
                foreach(var u in usuariosDto)
                {
                    usuariosDisponibles.Add(new UsuarioDisponibleViewModel
                    {
                        Id = u.Id,
                        Nombre = u.Nombre,
                        Apellido = u.Apellido,
                        NombreUsuario = u.NombreUsuario,
                        FotoPerfilUrl = u.FotoPerfilUrl,
                        AmigosEnComun = await _amigoService.GetAmigosEnComunCountAsync(userId, u.Id)
                    });
                }

                ViewBag.SearchQuery = query;
                return View(usuariosDisponibles);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al buscar usuarios.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un usuario para enviar la solicitud de amistad.";
                return RedirectToAction(nameof(Nueva));
            }

            try
            {
                await _solicitudService.SendSolicitudAsync(GetCurrentUserId(), id.Value);
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
            try
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
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud.";
                return RedirectToAction(nameof(Index));
            }
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
            try
            {
                var userId = GetCurrentUserId();
                var pendientes = await _solicitudService.GetSolicitudesRecibidasAsync(userId);
                var sol = pendientes.FirstOrDefault(s => s.Id == id);
                if (sol == null) return RedirectToAction(nameof(Index));
                var vm = new LinkUpPro.Presentation.ViewModels.Solicitud.SolicitudAmistadViewModel { Id = sol.Id, NombreUsuario = sol.Emisor.NombreUsuario };
                return View(vm);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud.";
                return RedirectToAction(nameof(Index));
            }
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
            try
            {
                var userId = GetCurrentUserId();
                var enviadas = await _solicitudService.GetSolicitudesEnviadasAsync(userId);
                var sol = enviadas.FirstOrDefault(s => s.Id == id);
                if (sol == null) return RedirectToAction(nameof(Index));
                var vm = new LinkUpPro.Presentation.ViewModels.Solicitud.SolicitudAmistadViewModel { Id = sol.Id, NombreUsuario = sol.Receptor.NombreUsuario };
                return View(vm);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud.";
                return RedirectToAction(nameof(Index));
            }
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
            try
            {
                var userId = GetCurrentUserId();
                var enviadas = await _solicitudService.GetSolicitudesEnviadasAsync(userId);
                var sol = enviadas.FirstOrDefault(s => s.Id == id);
                if (sol == null) return RedirectToAction(nameof(Index));
                var vm = new LinkUpPro.Presentation.ViewModels.Solicitud.SolicitudAmistadViewModel { Id = sol.Id, NombreUsuario = sol.Receptor.NombreUsuario };
                return View(vm);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud.";
                return RedirectToAction(nameof(Index));
            }
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
