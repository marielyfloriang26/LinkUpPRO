using LinkUpPro.Application.Interfaces.Shared;
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
        _fileService = fileService;
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



    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // 1. Manejo de archivo (foto)
        string fotoPath = string.Empty;
        if (vm.FotoPerfil != null)
        {
            fotoPath = _fileService.UploadFile(vm.FotoPerfil, "images/users");
        }

        // 2. Crear entidad de usuario
        var user = new Usuario 
        { 
            UserName = vm.NombreUsuario, 
            Email = vm.Correo, 
            Nombre = vm.Nombre, 
            Apellido = vm.Apellido,
            PhoneNumber = vm.Telefono,
            FotoPerfilUrl = fotoPath
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (result.Succeeded)
        {
            // 3. Generar Token de activación
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", 
                new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

            // 4. Enviar correo (Requerimiento de Capa Shared)
            await _emailService.SendAsync(vm.Correo, "Activación de Cuenta - LinkUp Pro", 
                $"Hola {vm.Nombre}, por favor activa tu cuenta haciendo clic en el siguiente enlace: <a href='{callbackUrl}'>Activar Cuenta</a>");

            TempData["Success"] = "Su cuenta fue creada correctamente. Hemos enviado un enlace de activación a su correo.";
            return RedirectToAction("RegisterConfirmation");
        }

        // 5. Mostrar errores de Identity (como nombre de usuario duplicado)
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(vm);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }



    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var user = await _userManager.FindByNameAsync(vm.NombreUsuario);
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, protocol: HttpContext.Request.Scheme);
            await _emailService.SendAsync(user.Email, "Restablecer contraseña", $"Para restablecer su contraseña haga clic aquí: <a href='{callbackUrl}'>Restablecer</a>");
        }
        ViewBag.Message = "Si el nombre de usuario corresponde a una cuenta registrada, recibirá un enlace para restablecer su contraseña.";
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
        // Retornamos una vista que contenga el token y el email (puedes usar un campo oculto)
        return View(new ResetPasswordViewModel { Token = token, Email = email });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user == null)
        {
            // Por seguridad, no revelamos si el usuario existe
            return RedirectToAction("Login");
        }

        var result = await _userManager.ResetPasswordAsync(user, vm.Token, vm.Password);
        if (result.Succeeded)
        {
            TempData["Success"] = "Su contraseña fue restablecida correctamente. Ya puede iniciar sesión.";
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
        return View(vm);
    }

    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            TempData["Success"] = "Su cuenta fue activada correctamente. Ya puede iniciar sesión.";
            return RedirectToAction("Login");
        }
        
        TempData["Error"] = "El enlace de activación no es válido o ya fue utilizado.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResendEmail()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEmail(string email)
    {
        // 1. Buscar el usuario por email
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            // Por seguridad, no decimos si el email existe o no, solo damos un mensaje genérico
            return View("ResendEmailConfirmation");
        }

        // 2. Generar el token de confirmación
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

        // 3. Enviar el correo (asegúrate de reutilizar tu servicio de email)
        await _emailService.SendAsync(user.Email, "Reenvío de Activación - LinkUp Pro", $"Haz clic aquí para activar tu cuenta: {callbackUrl}");

        return View("ResendEmailConfirmation");
    }







    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    /*[HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => View(new LoginViewModel());

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

        if (!user.EmailConfirmed)
        {
            ModelState.AddModelError("", "Su cuenta se encuentra inactiva. Debe activarla mediante el enlace enviado a su correo.");
            return View(vm);
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName, vm.Password, vm.MantenerSesion, lockoutOnFailure: true);

        if (result.Succeeded) return RedirectToAction("Index", "Home");
        
        if (result.IsLockedOut)
        {
            ModelState.AddModelError("", "La cuenta se encuentra bloqueada temporalmente. Inténtelo en 15 minutos.");
            return View(vm);
        }

        ModelState.AddModelError("", "El nombre de usuario o la contraseña son incorrectos.");
        return View(vm);
    }*/
}