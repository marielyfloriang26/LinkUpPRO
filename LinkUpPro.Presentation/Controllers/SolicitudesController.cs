using Microsoft.AspNetCore.Mvc;
using LinkUpPro.Presentation.ViewModels.Solicitud;
using System.Collections.Generic;

namespace LinkUpPro.Presentation.Controllers
{
    public class SolicitudesController : Controller
    {
        public SolicitudesController()
        {
        }

        public IActionResult Index()
        {
            var vm = new SolicitudesIndexViewModel
            {
                Pendientes = new List<SolicitudAmistadViewModel>(),
                Enviadas = new List<SolicitudAmistadViewModel>()
            };
            return View(vm);
        }

        public IActionResult Nueva(string query)
        {
            var usuariosDisponibles = new List<UsuarioDisponibleViewModel>();
            if (!string.IsNullOrEmpty(query))
            {
                usuariosDisponibles.Add(new UsuarioDisponibleViewModel { Id = "2", NombreCompleto = "Maria Gomez", NombreUsuario = "mariag", AmigosEnComun = 1, FotoPerfilUrl = "/img/default.png" });
            }
            ViewBag.SearchQuery = query;
            return View(usuariosDisponibles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Enviar(string id)
        {
            TempData["SuccessMessage"] = "Solicitud enviada correctamente.";
            return RedirectToAction(nameof(Nueva));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Aceptar(int id)
        {
            TempData["SuccessMessage"] = "Solicitud aceptada. Ahora son amigos.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Rechazar(int id)
        {
            TempData["SuccessMessage"] = "Solicitud rechazada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancelar(int id)
        {
            TempData["SuccessMessage"] = "Solicitud cancelada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarHistorial(int id)
        {
            TempData["SuccessMessage"] = "Historial eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
