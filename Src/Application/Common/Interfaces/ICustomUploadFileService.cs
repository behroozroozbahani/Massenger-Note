using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PortalCore.Application.Common.Interfaces
{
    public interface ICustomUploadFileService
    {
        void DeleteFile(string fileNameWithUploadFolder);
        void DeleteFile(string uploadFolder, string fileName);
        void DeleteFile(string fileName, string webRootPath, string uploadFolder);
        Task<(bool IsSaved, string DirectoryPath, string FilePath, string FileName)> UploadFileAsync(string fileDestinationDirectory, IFormFile file, string defaultFileName = "");
        Task<string> UploadFileAsync(IFormFile inputFile, string webRootPath, string uploadFolder);
    }
}
