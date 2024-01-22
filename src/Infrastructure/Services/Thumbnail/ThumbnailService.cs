using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PortalCore.Application.Common.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PortalCore.Infrastructure.Services.Thumbnail
{
    public class ThumbnailService : IThumbnailService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IGenerateFileNameService _generateFileNameService;

        public ThumbnailService(IWebHostEnvironment environment, IGenerateFileNameService generateFileNameService)
        {
            _environment = environment;
            _generateFileNameService = generateFileNameService;
        }

        public async Task<string> CreateThumbnailAsync(IFormFile inputFile, int width, string fileDestinationDirectory, string fileName, CancellationToken cancellationToken)
        {
            var image = await Image.LoadAsync(inputFile.OpenReadStream());

            var aspectRatio = image.Height / image.Width;

            image.Mutate(x => x.Resize(width, width * aspectRatio));

            var uploadsRootFolder = Path.Combine(_environment.WebRootPath, fileDestinationDirectory);
            if (!Directory.Exists(uploadsRootFolder))
            {
                Directory.CreateDirectory(uploadsRootFolder);
            }


            //Rename FileName
            var newFileName = _generateFileNameService.GenerateFileNameAsync();

            var extension = Path.GetExtension(fileName);
            //fileName = fileName.Replace(extension, "");
            fileName = $"{""}{newFileName}{extension}";
            var filePath = Path.Combine(uploadsRootFolder, fileName);
            await image.SaveAsync(filePath, cancellationToken);

            return fileName;
        }
    }
}
