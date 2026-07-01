using System;
using System.Threading.Tasks;
using LinkUpPro.Application.DTOs.Account;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Interfaces.Services.Interfaces;
using LinkUpPro.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace LinkUpPro.Infrastructure.Persistence.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IUploadFileService _uploadFileService;
    private readonly IEmailService _emailService;

    public AccountService(
        UserManager<Usuario> userManager, 
        SignInManager<Usuario> signInManager, 
        IUploadFileService uploadFileService, 
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _uploadFileService = uploadFileService;
        _emailService = emailService;
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return new AuthenticationResponse(false, "El nombre de usuario o la contraseña son incorrectos.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return new AuthenticationResponse(false, "La cuenta se encuentra bloqueada temporalmente debido a varios intentos fallidos. Inténtelo nuevamente en 15 minutos o restablezca su contraseña.");
        }

        if (!user.EsActivo)
        {
            return new AuthenticationResponse(false, "Su cuenta se encuentra inactiva. Debe activarla mediante el enlace enviado a su correo electrónico.");
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return new AuthenticationResponse(true, string.Empty);
        }

        if (result.IsLockedOut)
        {
            return new AuthenticationResponse(false, "La cuenta se encuentra bloqueada temporalmente debido a varios intentos fallidos. Inténtelo nuevamente en 15 minutos o restablezca su contraseña.");
        }

        return new AuthenticationResponse(false, "El nombre de usuario o la contraseña son incorrectos.");
    }

    public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null)
            return new RegisterResponse(false, "Este nombre de usuario ya se encuentra registrado.");

        var existingEmail = await _userManager.FindByEmailAsync(request.Correo);
        if (existingEmail != null)
            return new RegisterResponse(false, "Este correo electrónico ya se encuentra registrado.");

        string imageUrl = await _uploadFileService.UploadProfileImageAsync(request.FotoStream, request.NombreArchivo);

        var user = new Usuario
        {
            UserName = request.Username,
            Email = request.Correo,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            PhoneNumber = request.Telefono,
            FotoPerfilUrl = imageUrl,
            EsActivo = false,
            FechaRegistro = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) 
            return new RegisterResponse(false, "La contraseña no cumple las reglas de seguridad establecidas.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string callbackUrl = $"https://localhost:5232/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        await _emailService.SendAsync(user.Email, "Activación de Cuenta - LinkUpPro",
            $"Su cuenta fue creada correctamente. Active su cuenta haciendo clic aquí: <a href='{callbackUrl}'>Activar Cuenta</a>");

        return new RegisterResponse(true, "Su cuenta fue creada correctamente. Hemos enviado un enlace de activación a su correo electrónico.");
    }

    public async Task<string> ConfirmAccountAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return "El enlace de activación no es válido o ya fue utilizado.";

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return "El enlace de activación no es válido o ya fue utilizado.";

        user.EsActivo = true;
        await _userManager.UpdateAsync(user);
        return "Su cuenta fue activada correctamente. Ya puede iniciar sesión.";
    }

    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string callbackUrl = $"https://localhost:5232/Account/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendAsync(user.Email, "Restablecer Contraseña - LinkUpPro",
                $"Restablezca su contraseña ingresando a este enlace: <a href='{callbackUrl}'>Restablecer Contraseña</a>");
        }
        return "Si el nombre de usuario corresponds a una cuenta registrada, recibirá un enlace para restablecer su contraseña.";
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return "El token no es válido o ya ha sido utilizado.";

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded) return "El token no es válido o ya ha sido utilizado.";

        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.ResetAccessFailedCountAsync(user);
        await _userManager.UpdateSecurityStampAsync(user);

        return "Su contraseña fue restablecida correctamente. Ya puede iniciar sesión.";
    }

    public async Task<string> ResendVerificationEmailAsync(ResendVerificationEmailRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user != null && !user.EsActivo)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string callbackUrl = $"https://localhost:5232/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendAsync(user.Email, "Reenvío de Enlace de Activación",
                $"Use el siguiente enlace para activar su cuenta: <a href='{callbackUrl}'>Activar Cuenta</a>");
        }
        return "Si la cuenta existe y todavía no ha sido activada, recibirá un nuevo enlace de activación.";
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}