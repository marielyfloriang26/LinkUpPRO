using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Application.ViewModels.MiPerfil;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Interfaces.Shared;
using LinkUpPro.Application.DTOs.MiPerfilDTO;

namespace LinkUpPro.Presentation.Controllers;

[Authorize(Policy = "CuentaActiva")]
public class PerfilController : Controller
{
    private readonly IPerfilService _perfilService;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public PerfilController(
        IPerfilService perfilService,
        SignInManager<Usuario> signInManager,
        UserManager<Usuario> userManager,
        IFileService fileService,
        IMapper mapper)
    {
        _perfilService = perfilService;
        _signInManager = signInManager;
        _userManager = userManager;
        _fileService = fileService;
        _mapper = mapper;
    }

    // GET: /Perfil (Consulta - Solo lectura)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();

        var vm = _mapper.Map<PerfilViewModel>(user);
        return View(vm);
    }

    // GET: /Perfil/Editar (Formulario editable)
    [HttpGet]
    public async Task<IActionResult> Editar()
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();

        var vm = _mapper.Map<PerfilViewModel>(user);
        return View(vm);
    }

    // POST: /Perfil/Editar (Procesar cambios)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(PerfilViewModel vm)
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();

        // Limpieza y validaciones obligatorias del controlador
        vm.Nombre = vm.Nombre?.Trim() ?? string.Empty;
        vm.Apellido = vm.Apellido?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(vm.Nombre))
            ModelState.AddModelError("Nombre", "Debe ingresar su nombre.");

        if (string.IsNullOrWhiteSpace(vm.Apellido))
            ModelState.AddModelError("Apellido", "Debe ingresar su apellido.");

        if (vm.FotoPerfil != null)
        {
            var extension = Path.GetExtension(vm.FotoPerfil.FileName).ToLower();
            var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!extensionesValidas.Contains(extension))
                ModelState.AddModelError("FotoPerfil", "El archivo seleccionado no tiene un formato de imagen válido.");

            if (vm.FotoPerfil.Length > 5 * 1024 * 1024)
                ModelState.AddModelError("FotoPerfil", "La imagen seleccionada no puede superar los 5 MB.");
        }

        // Valida que si se completa un campo de contra, se completen los tres
        bool intentandoCambiarClave = !string.IsNullOrEmpty(vm.PasswordActual) || !string.IsNullOrEmpty(vm.PasswordNuevo) || !string.IsNullOrEmpty(vm.PasswordConfirmacion);

        if (intentandoCambiarClave)
        {
            if (string.IsNullOrEmpty(vm.PasswordActual) || string.IsNullOrEmpty(vm.PasswordNuevo) || string.IsNullOrEmpty(vm.PasswordConfirmacion))
            {
                var msg = "Para cambiar su contraseña debe completar la contraseña actual, la nueva contraseña y su confirmación.";
                ModelState.AddModelError("PasswordActual", msg);
                ModelState.AddModelError("PasswordNuevo", msg);
                ModelState.AddModelError("PasswordConfirmacion", msg);
            }
        }

        if (!ModelState.IsValid)
        {
            vm.NombreUsuario = user.UserName!;
            vm.Correo = user.Email!;
            vm.FotoPerfilUrl = user.FotoPerfilUrl;
            return View(vm);
        }

        // Subir fisicamente la foto nueva
        string? fotoVieja = user.FotoPerfilUrl;
        string? fotoNueva = null;

        try
        {
            if (vm.FotoPerfil != null)
            {
                fotoNueva = await _fileService.UploadFileAsync(vm.FotoPerfil, "users");
            }
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "No fue posible actualizar la foto de perfil. Inténtelo nuevamente.");
            vm.NombreUsuario = user.UserName!;
            vm.Correo = user.Email!;
            vm.FotoPerfilUrl = user.FotoPerfilUrl;
            return View(vm);
        }

        // Mapear al dto
        var dto = _mapper.Map<ModificarPerfilDto>(vm);
        dto.Id = userId;
        if (fotoNueva != null)
        {
            dto.FotoPerfilUrl = fotoNueva;
        }

        // Llamar al servicio
        var result = await _perfilService.ActualizarPerfilAsync(dto);

        if (result.Succeeded)
        {
            if (fotoNueva != null && !string.IsNullOrEmpty(fotoVieja))
            {
                _fileService.DeleteFile(fotoVieja);
            }

            if (result.PasswordChanged)
            {
                await _signInManager.SignOutAsync();
                TempData["Success"] = "Su perfil y contraseña fueron actualizados correctamente. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Su perfil fue actualizado correctamente.";
                return RedirectToAction("Index"); // Redirige a la pantalla de consulta de perfil
            }
        }
        else
        {
            if (fotoNueva != null)
            {
                _fileService.DeleteFile(fotoNueva);
            }

                foreach (var err in result.Errors)
            {
                // Mapea el error al campo especifico basandose en palabras clave del mensaje
                if (err.Contains("actual") || err.Contains("incorrecta"))
                {
                    ModelState.AddModelError("PasswordActual", err);
                }
                else if (err.Contains("confirmación") || err.Contains("coinciden"))
                {
                    ModelState.AddModelError("PasswordConfirmacion", err);
                }
                else if (err.Contains("diferente"))
                {
                    ModelState.AddModelError("PasswordNuevo", err);
                }
                else
                {
                    // Errores de fortaleza 
                    ModelState.AddModelError("PasswordNuevo", err);
                }
            }
        }

        vm.NombreUsuario = user.UserName!;
        vm.Correo = user.Email!;
        vm.FotoPerfilUrl = user.FotoPerfilUrl;
        return View(vm);
    }
}