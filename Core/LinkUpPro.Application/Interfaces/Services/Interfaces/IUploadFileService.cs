using System.IO;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Interfaces.Services.Interfaces;

public interface IUploadFileService
{
    Task<string> UploadProfileImageAsync(Stream fileStream, string fileName);
}