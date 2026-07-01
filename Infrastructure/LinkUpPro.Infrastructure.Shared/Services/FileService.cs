using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Interfaces.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace LinkUpPro.Infrastructure.Shared.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            return string.Empty;

        // Security Validations
        if (file.Length > 5 * 1024 * 1024) throw new Exception("Archivo muy grande");
        
        var ext = Path.GetExtension(file.FileName).ToLower();
        var permitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!permitidas.Contains(ext)) throw new Exception("Extensión no permitida");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "images", folderName);
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Nombre unico para evitar duplicados
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/images/{folderName}/{uniqueFileName}";
    }

    public void DeleteFile(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        var path = fileUrl.TrimStart('/');
        var absolutePath = Path.Combine(_env.WebRootPath, path.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
    }
}