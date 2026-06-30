using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinkUpPro.Application.DTOs.Publicacion;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.ViewModels.Publicacion;
using LinkUpPro.Application.DTOs.PublicacionDTO;

namespace LinkUpPro.Presentation.Controllers;

// [Authorize]
public class PublicacionController : Controller
{
    private readonly IPublicacionService _publicacionService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public PublicacionController(IPublicacionService publicacionService, IFileService fileService, IMapper mapper)
    {
        _publicacionService = publicacionService;
        _fileService = fileService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? textoBusqueda, string? tipoContenido, string? estadoEdicion, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde > fechaHasta)
        {
            ViewBag.ErrorFechas = "La fecha inicial no puede ser posterior a la fecha final.";

            // Retorna una lista vacia o previa para cumplir la restriccion visual inmediatamente
            return View(new List<PublicacionViewModel>());
        }
        var listaDtos = await _publicacionService.ObtenerTodasAsync(textoBusqueda, tipoContenido, estadoEdicion, fechaDesde, fechaHasta);

        var listaVm = _mapper.Map<List<PublicacionViewModel>>(listaDtos);

        //se mantiene el estado en el ViewBag para que los inputs de la vista parcial conserven lo escrito al recargar
        ViewBag.TextoBusqueda = textoBusqueda;
        ViewBag.TipoContenido = tipoContenido ?? "Todos";
        ViewBag.EstadoEdicion = estadoEdicion ?? "Todas";
        ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
        ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
       
        return View(listaVm);
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
            var listaDtos = await _publicacionService.ObtenerTodasAsync();
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

        // Simulación: Aquí debería capturar el ID del usuario autenticado mediante Identity
        // Ejemplo: crearDto.UsuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        
        crearDto.UsuarioId = 1; // !!!!!!!!!!!!!!
        // TEMPORAL Temporal para pruebas hasta que tu compañero monte el Login

        await _publicacionService.CrearAsync(crearDto);
        TempData["MensajeExito"] = "La publicación fue creada correctamente.";
        return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            
            var listaDtos = await _publicacionService.ObtenerTodasAsync();
            var listaVm = _mapper.Map<List<PublicacionViewModel>>(listaDtos);
            return View("Index", listaVm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(GuardarPublicacionViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        // Convierte el vm al dto de modificacion
        var modificarDto = _mapper.Map<ModificarPublicacionDto>(vm);

        await _publicacionService.EditarAsync(modificarDto);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        // Valida en el servidor la propiedad antes de eliminar
        await _publicacionService.EliminarAsync(id);
        
        return RedirectToAction(nameof(Index));
    }
}