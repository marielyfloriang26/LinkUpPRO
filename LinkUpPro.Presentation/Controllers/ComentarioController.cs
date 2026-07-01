
using Microsoft.AspNetCore.Mvc;
using LinkUpPro.Application.DTOs.ComentarioDTO;
using LinkUpPro.Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;

namespace LinkUpPro.Presentation.Controllers;

[Authorize(Policy = "CuentaActiva")]
public class ComentarioController : Controller
{
    private readonly IComentarioService _comentarioService;
    private readonly IAuthorizationService _authorizationService;

    public ComentarioController(IComentarioService comentarioService, IAuthorizationService authorizationService)
    {
        _comentarioService = comentarioService;
         _authorizationService = authorizationService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearComentarioDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("El formulario contiene datos inválidos.");
        }

        
        dto.UsuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

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
        var comentario = await _comentarioService.ObtenerPorIdAsync(id); 
        if (comentario == null) return NotFound();

        // Valida propiedad del comentario 
        var authResult = await _authorizationService.AuthorizeAsync(User, comentario, "PropietarioPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        int usuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

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
        var comentario = await _comentarioService.ObtenerPorIdAsync(id); 

        if (comentario == null) return NotFound();

        // Valida propiedad 
        var authResult = await _authorizationService.AuthorizeAsync(User, comentario, "PropietarioPolicy");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        int usuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

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