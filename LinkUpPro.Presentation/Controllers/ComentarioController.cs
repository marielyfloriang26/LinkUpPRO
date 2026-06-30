
using Microsoft.AspNetCore.Mvc;
using LinkUpPro.Application.DTOs.ComentarioDTO;
using LinkUpPro.Application.Interface.Services;

namespace LinkUpPro.Presentation.Controllers;

public class ComentarioController : Controller
{
    private readonly IComentarioService _comentarioService;

    public ComentarioController(IComentarioService comentarioService)
    {
        _comentarioService = comentarioService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearComentarioDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("El formulario contiene datos inválidos.");
        }

        // !!!!! Simulación: Asignamos el ID del usuario logueado (Mariely = 1)
        dto.UsuarioId = 1; 

        try
        {
            await _comentarioService.CrearAsync(dto);
            return Ok(); 
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, string contenido, int PublicacionId)
    {
        // !!!!! Simulación: Autor logueado ID 1
        int usuarioId = 1;

        try
        {
            await _comentarioService.EditarAsync(id, contenido, usuarioId);
            return Redirect($"/Publicacion/Index#comments-{PublicacionId}");
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
            return Redirect($"/Publicacion/Index#comments-{PublicacionId}");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id, int PublicacionId)
    {
        // !!!Simulación: Autor logueado ID 1
        int usuarioId = 1;

        try
        {
            await _comentarioService.EliminarAsync(id, usuarioId);
            return Redirect($"/Publicacion/Index#comments-{PublicacionId}");
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
            return Redirect($"/Publicacion/Index#comments-{PublicacionId}");
        }
    }
}