using System.IO;
using System.Threading.Tasks;
using LinkUpPro.Application.Account.ViewModels;
using LinkUpPro.Application.DTOs.Account;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Interfaces.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Presentation.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? message)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        if (!string.IsNullOrEmpty(message)) ModelState.AddModelError(string.Empty, message);
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid) return View(vm);

        var result = await _accountService.AuthenticateAsync(new AuthenticationRequest(vm.NombreUsuario, vm.Contraseña, vm.MantenerSesionIniciada));
        if (result.IsSuccess) return RedirectToAction("Index", "Home");

        ModelState.AddModelError(string.Empty, result.Message);
        return View(vm);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (vm.FotoPerfil != null)
        {
            var ext = Path.GetExtension(vm.FotoPerfil.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp")
                ModelState.AddModelError("FotoPerfil", "Solo se permiten imágenes .jpg, .jpeg, .png o .webp.");
        }

        if (!ModelState.IsValid) return View(vm);

        using var stream = vm.FotoPerfil!.OpenReadStream();
        var request = new RegisterRequest(vm.Nombre, vm.Apellido, vm.Telefono, vm.Correo, vm.NombreUsuario, vm.Contraseña, stream, vm.FotoPerfil.FileName);

        var result = await _accountService.RegisterUserAsync(request);
        if (result.IsSuccess)
        {
            ViewBag.Message = result.Message;
            return View("SuccessInfo");
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(vm);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var msg = await _accountService.ConfirmAccountAsync(userId, token);
        return RedirectToAction("Login", new { message = msg });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            ModelState.AddModelError(string.Empty, "El nombre de usuario es requerido.");
            return View();
        }
        var msg = await _accountService.ForgotPasswordAsync(new ForgotPasswordRequest(username));
        ViewBag.Message = msg;
        return View("SuccessInfo");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResetPassword(string userId, string token) => View(new ResetPasswordViewModel { UserId = userId, Token = token });

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var msg = await _accountService.ResetPasswordAsync(new ResetPasswordRequest(vm.UserId, vm.Token, vm.Contraseña));
        return RedirectToAction("Login", new { message = msg });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResendActivation() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ResendActivation(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            ModelState.AddModelError(string.Empty, "El nombre de usuario es requerido.");
            return View();
        }
        var msg = await _accountService.ResendVerificationEmailAsync(new ResendVerificationEmailRequest(username));
        ViewBag.Message = msg;
        return View("SuccessInfo");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _accountService.SignOutAsync();
        return RedirectToAction("Login");
    }
}