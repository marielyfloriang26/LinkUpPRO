using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinkUpPro.Application.DTOs.Publicacion;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.ViewModels.Publicacion;
using LinkUpPro.Application.DTOs.PublicacionDTO;
using LinkUpPro.Application.Interfaces.Shared;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace LinkUpPro.Presentation.Controllers;

[Authorize(Policy = "CuentaActiva")]
public class PublicacionController : Controller
{
    private readonly IPublicacionService _publicacionService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public PublicacionController(IPublicacionService publicacionService, IFileService fileService, IMapper mapper, IAuthorizationService authorizationService)
    {
        _publicacionService = publicacionService;
        _fileService = fileService;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? textoBusqueda, string? tipoContenido, string? estadoEdicion, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        int usuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

        if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde > fechaHasta)
        {
            ViewBag.ErrorFechas = "La fecha inicial no puede ser posterior a la fecha final.";
 
            // Cargar publicaciones normales ignorando las fechas erróneas
            var listaDtos = await _publicacionService.ObtenerTodasAsync(usuarioId, textoBusqueda, tipoContenido, estadoEdicion);
            var listaVm = _mapper.Map<List<PublicacionViewModel>>(listaDtos);
            
            return View(listaVm);
        }
       
        var listaDtosNormal = await _publicacionService.ObtenerTodasAsync(usuarioId, textoBusqueda, tipoContenido, estadoEdicion, fechaDesde, fechaHasta);
        var listaVmNormal = _mapper.Map<List<PublicacionViewModel>>(listaDtosNormal);

        //se mantiene el estado en el ViewBag para que los inputs de la vista parcial conserven lo escrito al recargar
        ViewBag.TextoBusqueda = textoBusqueda;
        ViewBag.TipoContenido = tipoContenido ?? "Todos";
        ViewBag.EstadoEdicion = estadoEdicion ?? "Todas";
        ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
        ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
       
        return View(listaVmNormal);
    }


    [HttpPost]
    [ValidateAntiForgeryToken] 
    public async Task<IActionResult> Crear(GuardarPublicacionViewModel vm, string TipoContenido)
    {
        if (TipoContenido == "Imagen")
        {
            if (vm.ImagenArchivo == null || vm.ImagenArchivo.Length == 0)
            {
                ModelState.AddModelError("ImagenArchivo", "Debe seleccionar una imagen para crear la publicación.");
            }
            else
            {
                var extension = Path.GetExtension(vm.ImagenArchivo.FileName).ToLower();
                var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                
                if (!extensionesValidas.Contains(extension))
                {
                    ModelState.AddModelError("ImagenArchivo", "El archivo seleccionado no tiene un formato de imagen válido.");
                }

                if (vm.ImagenArchivo.Length > (5 * 1024 * 1024)) // 5MB
                {
                    ModelState.AddModelError("ImagenArchivo", "La imagen no debe superar los 5 MB.");
                }
            }
            vm.YouTubeVideoUrl = null;
        }
        else if (TipoContenido == "Video")
        {
            if (string.IsNullOrEmpty(vm.YouTubeVideoUrl) || string.IsNullOrWhiteSpace(vm.YouTubeVideoUrl))
            {
                ModelState.AddModelError("YouTubeVideoUrl", "Debe ingresar un enlace válido de YouTube.");
            }
            vm.ImagenArchivo = null;
        }
        
        if (!ModelState.IsValid)
        {
            // Si hay errores, vuelve a cargar el feed mostrando los errores del formulario
            int usuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var listaDtos = await _publicacionService.ObtenerTodasAsync(usuarioId);
            var listaVm = _mapper.Map<List<PublicacionViewModel>>(listaDtos);
            return View("Index", listaVm);
        }

        try
        {
            var crearDto = _mapper.Map<CrearPublicacionDto>(vm);
            
            // Subir el archivo fisicamente si es Tipo Imagen
            if (TipoContenido == "Imagen" && vm.ImagenArchivo != null)
            {
                string rutaImagen = await _fileService.UploadFileAsync(vm.ImagenArchivo, "publicaciones");
                crearDto.ImagenUrl = rutaImagen;
            }

            // captura el ID del usuario autenticado mediante Identity
            crearDto.UsuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            await _publicacionService.CrearAsync(crearDto);
            TempData["MensajeExito"] = "La publicación fue creada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            
            int usuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var listaDtos = await _publicacionService.ObtenerTodasAsync(usuarioId);
            var listaVm = _mapper.Map<List<PublicacionViewModel>>(listaDtos);
            return View("Index", listaVm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var publicacionDto = await _publicacionService.ObtenerPorIdAsync(id);
        if (publicacionDto == null)
        {
            return NotFound();
        }

        // Validacion de permisos de autor
        var authResult = await _authorizationService.AuthorizeAsync(User, publicacionDto, "PropietarioPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        var vm = _mapper.Map<GuardarPublicacionViewModel>(publicacionDto);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(GuardarPublicacionViewModel vm, string TipoContenido)
    {
        // Valida que la publi exista y que el usuario sea el dueno
        var publicacionOriginal = await _publicacionService.ObtenerPorIdAsync(vm.Id);
        if (publicacionOriginal == null) return NotFound();
        
        var authResult = await _authorizationService.AuthorizeAsync(User, publicacionOriginal, "PropietarioPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        if (TipoContenido == "Imagen")
        {
            // Solo valida archivo si el usuario carga uno nuevo
            if (vm.ImagenArchivo != null)
            {
                var extension = Path.GetExtension(vm.ImagenArchivo.FileName).ToLower();
                var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                
                if (!extensionesValidas.Contains(extension))
                {
                    ModelState.AddModelError("ImagenArchivo", "El archivo seleccionado no tiene un formato de imagen válido.");
                }

                if (vm.ImagenArchivo.Length > (5 * 1024 * 1024))
                {
                    ModelState.AddModelError("ImagenArchivo", "La imagen no debe superar los 5 MB.");
                }
            }
            vm.YouTubeVideoUrl = null;
        }
        else if (TipoContenido == "Video")
        {
            if (string.IsNullOrEmpty(vm.YouTubeVideoUrl) || string.IsNullOrWhiteSpace(vm.YouTubeVideoUrl))
            {
                ModelState.AddModelError("YouTubeVideoUrl", "Debe ingresar un enlace válido de YouTube.");
            }
            vm.ImagenArchivo = null;
            vm.ImagenUrl = null; // Limpia imagen vieja en el vm
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var modificarDto = _mapper.Map<ModificarPublicacionDto>(vm);

            // Si hay una nueva imagen, se sube y actualiza
            if (TipoContenido == "Imagen" && vm.ImagenArchivo != null)
            {
                string rutaImagen = await _fileService.UploadFileAsync(vm.ImagenArchivo, "publicaciones");
                modificarDto.ImagenUrl = rutaImagen;
            }
            else if (TipoContenido == "Imagen")
            {
                // Si no sube una nueva, mantiene la que ya tenia
                modificarDto.ImagenUrl = publicacionOriginal.ImagenUrl;
            }

            await _publicacionService.EditarAsync(modificarDto);
            TempData["MensajeExito"] = "La publicación fue actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        var publicacionDto = await _publicacionService.ObtenerPorIdAsync(id);
        if (publicacionDto == null)
        {
            return NotFound();
        }

        // Valida permisos del autor 
        var authResult = await _authorizationService.AuthorizeAsync(User, publicacionDto, "PropietarioPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        // Mapea a vm para enviarlo a la vista de confirmacion
        var vm = _mapper.Map<PublicacionViewModel>(publicacionDto);
        return View(vm);
    }

    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var publicacion = await _publicacionService.ObtenerPorIdAsync(id);
        if (publicacion == null) return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, publicacion, "PropietarioPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        await _publicacionService.EliminarAsync(id);
        
        return RedirectToAction(nameof(Index));
    }
}