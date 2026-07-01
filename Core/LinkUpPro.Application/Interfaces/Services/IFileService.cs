using Microsoft.AspNetCore.Http;

namespace LinkUpPro.Application.Interfaces.Services;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName);
    void DeleteFile(string fileUrl);
}