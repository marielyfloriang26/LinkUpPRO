using System.IO;
using System.Threading.Tasks;
using LinkUpPro.Application.Account.ViewModels;
using LinkUpPro.Application.DTOs.Account;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Interfaces.Services.Interfaces;
using LinkUpPro.Application.ViewModels.User;
using LinkUpPro.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Presentation.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IFileService _fileService;

    public AccountController(
        UserManager<Usuario> userManager, 
        SignInManager<Usuario> signInManager,
        IEmailService emailService,
        IFileService fileService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _file_service = fileService;
    }



    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() 
    {
        if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.FindByNameAsync(vm.NombreUsuario);
        if (user == null)
        {
            ModelState.AddModelError("", "El nombre de usuario o la contraseña son incorrectos.");
            return View(vm);
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            ModelState.AddModelError("", "Su cuenta se encuentra inactiva. Debe activarla mediante el enlace enviado a su correo electrónico.");
            return View(vm);
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName, vm.Password, vm.MantenerSesion, lockoutOnFailure: true);

        if (result.Succeeded) return RedirectToAction("Index", "Home");
        
        if (result.IsLockedOut)
        {
            ModelState.AddModelError("", "La cuenta se encuentra bloqueada temporalmente debido a varios intentos fallidos. Inténtelo nuevamente en 15 minutos o restablezca su contraseña.");
            return View(vm);
        }

        ModelState.AddModelError("", "El nombre de usuario o la contraseña son incorrectos.");
        return View(vm);
    }

    // ... (rest of our Identity-based actions kept unchanged) 
}

}