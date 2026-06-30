
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
}