using LinkUpPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

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


        var uploadsFolder = Path.Combine(_env.WebRootPath, "images", folderName);
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Nombre unico para evitar duplicados
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
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