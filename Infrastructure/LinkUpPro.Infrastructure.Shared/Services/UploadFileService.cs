using System;
using System.IO;
using System.Threading.Tasks;
using LinkUpPro.Application.Interfaces.Services.Interfaces;

namespace LinkUpPro.Infrastructure.Shared.Services;

public class UploadFileService : IUploadFileService
{
    public async Task<string> UploadProfileImageAsync(Stream fileStream, string fileName)
    {
        if (fileStream == null || fileStream.Length == 0) 
            return string.Empty;

        var extension = Path.GetExtension(fileName).ToLower();
        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".webp")
        {
            throw new Exception("Formato de imagen no permitido. Solo se aceptan .jpg, .jpeg, .png o .webp.");
        }

        // Límite estricto de 5 MB
        if (fileStream.Length > 5 * 1024 * 1024)
        {
            throw new Exception("La imagen supera el límite permitido de 5 MB.");
        }

        // Define la ruta física: wwwroot/images/profiles
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
        
        if (!Directory.Exists(folderPath)) 
            Directory.CreateDirectory(folderPath);

        // Genera un nombre único con GUID para evitar que dos usuarios con una foto llamada "foto.jpg" se sobreescriban
        string uniqueFileName = $"{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(folderPath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        // Retorna la URL relativa para guardarla en la base de datos
        return $"/images/profiles/{uniqueFileName}";
    }
}