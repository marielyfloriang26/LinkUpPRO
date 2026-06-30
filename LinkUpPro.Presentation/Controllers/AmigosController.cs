using Microsoft.AspNetCore.Mvc;
using LinkUpPro.Presentation.ViewModels.Amigo;
using System.Collections.Generic;

namespace LinkUpPro.Presentation.Controllers
{
    public class AmigosController : Controller
    {
        // Here we would inject the IAmigoService
        public AmigosController()
        {
        }

        public IActionResult Index()
        {
            var vm = new AmigoListViewModel
            {
                Amigos = new List<AmigoViewModel>
                {
                    new AmigoViewModel { Id = "1", NombreCompleto = "Juan Perez", NombreUsuario = "juanp", AmigosEnComun = 2, FotoPerfilUrl = "/img/default.png", Profesion = "Developer" }
                },
                TotalAmigos = 1
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Buscar(string query)
        {
            var vm = new AmigoListViewModel
            {
                SearchQuery = query,
                Amigos = new List<AmigoViewModel>(),
                TotalAmigos = 0
            };
            return View("Index", vm);
        }

        [HttpGet]
        public IActionResult Eliminar(string id)
        {
            var vm = new AmigoViewModel { Id = id, NombreCompleto = "Juan Perez" }; // Mock data
            return View(vm);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(string id)
        {
            // Call service to delete friend
            TempData["SuccessMessage"] = "Amigo eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
