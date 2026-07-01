// LinkUpPro.Application/Interfaces/Shared/IFileService.cs
using Microsoft.AspNetCore.Http;

namespace LinkUpPro.Application.Interfaces.Shared;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName);
    void DeleteFile(string fileUrl);
}
