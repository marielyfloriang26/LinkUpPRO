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
    private readonly IMapper _mapper;

    public PublicacionController(IPublicacionService publicacionService, IMapper mapper)
    {
        _publicacionService = publicacionService;
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
    public async Task<IActionResult> Crear(GuardarPublicacionViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            // Si hay errores, vuelve a cargar el feed mostrando los errores del formulario
            var listaDtos = await _publicacionService.ObtenerTodasAsync();
            var listaVm = _mapper.Map<List<PublicacionViewModel>>(listaDtos);
            return View("Index", listaVm);
        }

        // TODO: Aquí invocará el servicio de la capa Shared para guardar la imagen físicamente si vm.ImagenStream no es nulo.
        // Por ahora, mapeamos el ViewModel directamente al CrearPublicacionDto
        var crearDto = _mapper.Map<CrearPublicacionDto>(vm);
        
        // Simulación: Aquí debería capturar el ID del usuario autenticado mediante Identity
        // Ejemplo: crearDto.UsuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        crearDto.UsuarioId = 1; // !!!!!!!!!!!!!!
        // TEMPORAL Temporal para pruebas hasta que tu compañero monte el Login

        await _publicacionService.CrearAsync(crearDto);

        return RedirectToAction(nameof(Index));
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