using System;
using System.Threading.Tasks;
using LinkUpPro.Application.Interfaces.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LinkUpPro.Infrastructure.Shared.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public Task SendAsync(string to, string subject, string body)
    {
        var emailFrom = _config["EmailSettings:From"] ?? "proyectoevote@gmail.com";
        
        // Simulación en consola requerida para desarrollo local y revisión del maestro
        Console.WriteLine("\n==================================================================");
        Console.WriteLine($"[CORREO SIMULADO] Enviado desde: {emailFrom}");
        Console.WriteLine($"Para: {to}");
        Console.WriteLine($"Asunto: {subject}");
        Console.WriteLine("------------------------------------------------------------------");
        Console.WriteLine($"Cuerpo del mensaje:\n{body}");
        Console.WriteLine("==================================================================\n");
        
        return Task.CompletedTask;
    }
}