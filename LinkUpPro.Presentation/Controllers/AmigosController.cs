using Microsoft.AspNetCore.Mvc;
using LinkUpPro.Presentation.ViewModels.Amigo;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LinkUpPro.Application.Interfaces.Services;
using System.Security.Claims;
using System.Linq;
using LinkUpPro.Application.Exceptions;

namespace LinkUpPro.Presentation.Controllers
{
    [Authorize(Policy = "CuentaActiva")]
    public class AmigosController : Controller
    {
        private readonly IAmigoService _amigoService;
        private readonly IPublicacionService _publicacionService;
        private readonly AutoMapper.IMapper _mapper;

        public AmigosController(IAmigoService amigoService, IPublicacionService publicacionService, AutoMapper.IMapper mapper)
        {
            _amigoService = amigoService;
            _publicacionService = publicacionService;
            _mapper = mapper;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string query,
            string? textoBusquedaPub,
            int? amigoIdPub,
            string? tipoContenidoPub,
            DateTime? fechaDesdePub,
            DateTime? fechaHastaPub,
            string? estadoEdicionPub)
        {
            var userId = GetCurrentUserId();
            var amigosDto = string.IsNullOrWhiteSpace(query) 
                ? await _amigoService.GetAmigosAsync(userId) 
                : await _amigoService.BuscarAmigosAsync(userId, query);

            var vm = new AmigoListViewModel
            {
                Amigos = new List<AmigoViewModel>(),
                TotalAmigos = (await _amigoService.GetAmigosAsync(userId)).Count,
                SearchQuery = query,
                TextoBusquedaPub = textoBusquedaPub,
                AmigoIdPub = amigoIdPub,
                TipoContenidoPub = tipoContenidoPub ?? "Todos",
                FechaDesdePub = fechaDesdePub,
                FechaHastaPub = fechaHastaPub,
                EstadoEdicionPub = estadoEdicionPub ?? "Todas"
            };

            foreach (var a in amigosDto)
            {
                vm.Amigos.Add(new AmigoViewModel
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Apellido = a.Apellido,
                    NombreUsuario = a.NombreUsuario,
                    FotoPerfilUrl = a.FotoPerfilUrl,
                    AmigosEnComun = await _amigoService.GetAmigosEnComunCountAsync(userId, a.Id)
                });
            }

            // Validations for dates
            if (fechaDesdePub.HasValue && fechaHastaPub.HasValue && fechaDesdePub > fechaHastaPub)
            {
                ViewBag.ErrorFechas = "La fecha inicial no puede ser posterior a la fecha final.";
            }
            else
            {
                var pubDtos = await _publicacionService.ObtenerPublicacionesAmigosAsync(userId, textoBusquedaPub, amigoIdPub, tipoContenidoPub, estadoEdicionPub, fechaDesdePub, fechaHastaPub);
                vm.Publicaciones = _mapper.Map<List<LinkUpPro.Application.ViewModels.Publicacion.PublicacionViewModel>>(pubDtos);
            }

            // Publicaciones disponibles should be the total active publications of friends regardless of text/date filters
            var todasPubs = await _publicacionService.ObtenerPublicacionesAmigosAsync(userId);
            vm.PublicacionesDisponibles = todasPubs.Count;

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AmigosEnComun(int id)
        {
            var userId = GetCurrentUserId();
            var mutualFriends = await _amigoService.GetAmigosEnComunAsync(userId, id);
            return View(mutualFriends);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = GetCurrentUserId();
            var amigos = await _amigoService.GetAmigosAsync(userId);
            var amigo = amigos.FirstOrDefault(a => a.Id == id);

            if (amigo == null)
            {
                TempData["ErrorMessage"] = "La amistad seleccionada ya no se encuentra disponible.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new AmigoViewModel 
            { 
                Id = amigo.Id, 
                Nombre = amigo.Nombre,
                Apellido = amigo.Apellido,
                NombreUsuario = amigo.NombreUsuario
            }; 
            return View(vm);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _amigoService.DeleteAmigoAsync(userId, id);
                TempData["SuccessMessage"] = "La amistad fue eliminada correctamente.";
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
