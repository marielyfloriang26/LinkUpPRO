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
           
            string referer = Request.Headers["Referer"].ToString();
            return Redirect(string.IsNullOrEmpty(referer) ? $"/Publicacion/Index#post-{publicacionId}" : referer);
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
            string referer = Request.Headers["Referer"].ToString();
            return Redirect(string.IsNullOrEmpty(referer) ? "/Publicacion/Index" : referer);
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
            
            string referer = Request.Headers["Referer"].ToString();
            return Redirect(string.IsNullOrEmpty(referer) ? $"/Publicacion/Index#post-{publicacionId}" : referer);
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
            string referer = Request.Headers["Referer"].ToString();
            return Redirect(string.IsNullOrEmpty(referer) ? "/Publicacion/Index" : referer);
        }
    }
}