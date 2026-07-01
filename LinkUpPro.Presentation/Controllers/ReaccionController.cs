using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinkUpPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace LinkUpPro.Presentation.Controllers;

[Authorize(Policy = "CuentaActiva")]
public class ReaccionController : Controller
{
    private readonly IReaccionService _reaccionService;

    public ReaccionController(IReaccionService reaccionService)
    {
        _reaccionService = reaccionService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reaccionar(int publicacionId, string tipoReaccion)
    {
        int usuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

        try
        {
            await _reaccionService.ReaccionarAsync(publicacionId, usuarioId, tipoReaccion);
           
            return Redirect($"/Publicacion/Index#post-{publicacionId}");
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
            return RedirectToAction("Index", "Publicacion");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarReaccion(int publicacionId)
    {
        int usuarioId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

        try
        {
            await _reaccionService.EliminarReaccionAsync(publicacionId, usuarioId);
            
            return Redirect($"/Publicacion/Index#post-{publicacionId}");
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
            return RedirectToAction("Index", "Publicacion");
        }
    }
}