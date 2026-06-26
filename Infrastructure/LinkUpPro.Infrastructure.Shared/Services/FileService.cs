// LinkUpPro.Infrastructure.Shared/Services/FileService.cs
using LinkUpPro.Application.Interfaces.Shared;
using Microsoft.AspNetCore.Http;
using System.IO;

public class FileService : IFileService
{
    public string UploadFile(IFormFile file, string folderPath)
    {
        if (file == null || file.Length == 0) return string.Empty;

        // Crear carpeta si no existe
        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        // Generar nombre único para evitar colisiones
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        string fullPath = Path.Combine(path, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        // Retornamos la ruta relativa para guardarla en la BD
        return Path.Combine(folderPath, fileName).Replace("\\", "/");
    }
}