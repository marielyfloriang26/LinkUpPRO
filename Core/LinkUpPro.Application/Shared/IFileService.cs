// LinkUpPro.Application/Interfaces/Shared/IFileService.cs
using Microsoft.AspNetCore.Http;

namespace LinkUpPro.Application.Interfaces.Shared;

public interface IFileService
{
    string UploadFile(IFormFile file, string folderPath);
}