using PortalCore.Application.Common.Interfaces;
using PortalCore.Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PortalCore.Infrastructure.Services
{
    public class CustomUploadFileService : ICustomUploadFileService
    {
        private const int MaxBufferSize = 0x10000; // 64K. The artificial constraint due to win32 api limitations. Increasing the buffer size beyond 64k will not help in any circumstance, as the underlying SMB protocol does not support buffer lengths beyond 64k.

        private readonly IWebHostEnvironment _environment;
        private readonly IGenerateFileNameService _generateFileNameService;


        public CustomUploadFileService(IWebHostEnvironment environment, IGenerateFileNameService generateFileNameService)
        {
            _environment = environment;
            _generateFileNameService = generateFileNameService;
        }

        public void DeleteFile(string fileNameWithUploadFolder)
        {
            var path = Path.Combine(_environment.WebRootPath, fileNameWithUploadFolder);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteFile(string uploadFolder, string fileName)
        {
            var path = Path.Combine(_environment.WebRootPath, uploadFolder, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteFile(string fileName, string webRootPath, string uploadFolder)
        {
            var path = Path.Combine(webRootPath, uploadFolder, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public async Task<(bool IsSaved, string DirectoryPath, string FilePath, string FileName)> UploadFileAsync(string fileDestinationDirectory, IFormFile file,  string defaultFileName = "")
        {
            //Rename FileName
            var newFileName = _generateFileNameService.GenerateFileNameAsync() + Path.GetExtension(file.FileName);

            var uploadsRootFolder = Path.Combine(_environment.WebRootPath, fileDestinationDirectory);
            if (!Directory.Exists(uploadsRootFolder))
            {
                Directory.CreateDirectory(uploadsRootFolder);
            }
            
            var fileName = string.IsNullOrEmpty(defaultFileName)? newFileName : defaultFileName;
            var filePath = Path.Combine(uploadsRootFolder, fileName);
            var directoryPath = string.Join("/", fileDestinationDirectory, fileName);
            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
                FileShare.None,
                bufferSize: MaxBufferSize,
                // you have to explicitly open the FileStream as asynchronous
                // or else you're just doing synchronous operations on a background thread.
                useAsync: true);
            await file.CopyToAsync(fileStream);

            return (true, directoryPath, filePath, fileName);
        }

        public async Task<string> UploadFileAsync(IFormFile inputFile, string webRootPath, string uploadFolder)
        {
            createUploadDir(webRootPath, uploadFolder);
            var (fileName, imageFilePath) = getOutputFileInfo(inputFile, webRootPath, uploadFolder);

            using (var fileStream = new FileStream(
                imageFilePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: MaxBufferSize,
                // you have to explicitly open the FileStream as asynchronous
                // or else you're just doing synchronous operations on a background thread.
                useAsync: true))
            {
                await inputFile.CopyToAsync(fileStream);
            }

            return $"{uploadFolder}/{fileName}";
        }

        private static (string FileName, string FilePath) getOutputFileInfo(IFormFile inputFile, string webRootPath, string uploadFolder)
        {
            var fileName = Path.GetFileName(inputFile.Name);
            var imageFilePath = Path.Combine(webRootPath, uploadFolder, fileName);
            if (File.Exists(imageFilePath))
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var fileExtension = Path.GetExtension(fileName);
                fileName = $"{fileNameWithoutExtension.RemoveIllegalCharacters()}-{Guid.NewGuid()}{fileExtension}";
                imageFilePath = Path.Combine(webRootPath, uploadFolder, fileName);
            }
            return (fileName, imageFilePath);
        }

        private static void createUploadDir(string webRootPath, string uploadFolder)
        {
            var folderDirectory = Path.Combine(webRootPath, uploadFolder);
            if (!Directory.Exists(folderDirectory))
            {
                Directory.CreateDirectory(folderDirectory);
            }
        }
    }
}
