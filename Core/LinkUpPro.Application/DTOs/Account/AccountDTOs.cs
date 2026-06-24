using System.IO;

namespace LinkUpPro.Application.DTOs.Account;

// Datos requeridos para iniciar sesión
public record AuthenticationRequest(string Username, string Password, bool RememberMe);
public record AuthenticationResponse(bool IsSuccess, string Message);

// Datos requeridos para registrar un usuario (Usamos Stream para desacoplarnos de HTTP)
public record RegisterRequest(
    string Nombre, 
    string Apellido, 
    string Telefono, 
    string Correo, 
    string Username, 
    string Password, 
    Stream FotoStream, 
    string NombreArchivo
);
public record RegisterResponse(bool IsSuccess, string Message);

// Datos para la recuperación y activación de cuentas
public record ForgotPasswordRequest(string Username);
public record ResetPasswordRequest(string UserId, string Token, string NewPassword);
public record ResendVerificationEmailRequest(string Username);