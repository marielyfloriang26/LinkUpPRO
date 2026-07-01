using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Presentation.ViewModels.Perfil;
using AutoMapper;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LinkUpPro.Presentation.Controllers
{
    [Authorize(Policy = "CuentaActiva")]
    public class PerfilController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IAmigoService _amigoService;
        private readonly IAmistadRepository _amistadRepository;
        private readonly IPublicacionService _publicacionService;
        private readonly IMapper _mapper;

        public PerfilController(
            UserManager<Usuario> userManager,
            IAmigoService amigoService,
            IAmistadRepository amistadRepository,
            IPublicacionService publicacionService,
            IMapper mapper)
        {
            _userManager = userManager;
            _amigoService = amigoService;
            _amistadRepository = amistadRepository;
            _publicacionService = publicacionService;
            _mapper = mapper;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> Ver(string username)
        {
            var userId = GetCurrentUserId();
            var targetUser = await _userManager.FindByNameAsync(username);
            
            if (targetUser == null || !targetUser.EmailConfirmed)
            {
                TempData["ErrorMessage"] = "No posee permisos para visualizar el perfil de este usuario.";
                return RedirectToAction("Index", "Amigos");
            }

            // Check if active friendship exists
            var friendship = await _amistadRepository.GetAmistadEntreUsuariosAsync(userId, targetUser.Id);
            if (friendship == null || friendship.Estado != "Activa")
            {
                TempData["ErrorMessage"] = "No posee permisos para visualizar el perfil de este usuario.";
                return RedirectToAction("Index", "Amigos");
            }

            var pubDtos = await _publicacionService.ObtenerPublicacionesAmigosAsync(userId, amigoId: targetUser.Id);
            var publicationsVm = _mapper.Map<List<LinkUpPro.Application.ViewModels.Publicacion.PublicacionViewModel>>(pubDtos);

            var vm = new PerfilAmigoViewModel
            {
                Id = targetUser.Id,
                Nombre = targetUser.Nombre,
                Apellido = targetUser.Apellido,
                NombreUsuario = targetUser.UserName ?? "",
                FotoPerfilUrl = targetUser.FotoPerfilUrl,
                AmigosEnComun = await _amigoService.GetAmigosEnComunCountAsync(userId, targetUser.Id),
                Publicaciones = publicationsVm
            };

            return View(vm);
        }
    }
}
