using Microsoft.AspNetCore.Http;
using PortalCore.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace PortalCore.Infrastructure.Services.MessengerCheckImages
{
    public class MessengerCheckImagesService : IMessengerCheckImagesService
    {
        public static readonly List<string> imagesFormat = new List<string>
        {
            ".bmp", ".cod", ".gif", ".ief", ".jpe", ".jpeg", ".jpg", ".jfif", ".svg", ".tif",
            ".tiff", ".ras", ".cmx", ".ico", ".pnm", ".pbm", ".pgm", ".ppm", ".rgb", ".xbm",
            ".xpm", ".xwd"
        };

        public static readonly List<string> imagesMimeFormat = new List<string>
        {
            "image/bmp", "image/cis-cod", "image/gif", "image/ief", "image/jpeg", "image/pipeg", "image/svg+xml",
            "image/tiff", "image/x-cmu-raster", "image/x-cmx", "image/x-icon", "image/x-portable-anymap",
            "image/x-portable-bitmap", "image/x-portable-graymap", "image/x-portable-pixmap", "image/x-rgb",
            "image/x-xbitmap", "image/x-xpixmap", "image/x-xwindowdump"
        };

        public void CheckFileAsync(IFormFile inputFile, CancellationToken cancellationToken)
        {
            string imageExtention = Path.GetExtension(inputFile.FileName);

            string imageMime = MimeKit.MimeTypes.GetMimeType(inputFile.FileName);

            if (!imagesFormat.Any(imageExtention.Contains) && !imagesMimeFormat.Any(imageMime.Contains))
            {
                throw new Exception("مجاز به بارگذاری نیستید");
            }
        }

        public string ChechFileMultipelExtension(string fileName, CancellationToken cancellationToken)
        {
            string imageExtention = Path.GetExtension(fileName);

            while (!string.IsNullOrEmpty(imageExtention))
            {
                var extention = imageExtention;  
                fileName = fileName.Replace(imageExtention, "");
                imageExtention = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(imageExtention))
                {
                    fileName += extention;
                    break;
                }
            }

            return fileName;
        }
    }
}
