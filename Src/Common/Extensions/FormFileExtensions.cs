using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace PortalCore.Common.Extensions
{
    public static class FormFileExtensions
    {
        public static string GetUniqueFileName(this IFormFile formFile)
        {
            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
            var extension = Path.GetExtension(formFile.FileName);
            return $"{fileName.RemoveIllegalCharacters()}.{DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture)}.{Guid.NewGuid().ToString("N")}{extension}";
        }

        public static (string DirectoryPath, string FilePath) GetUniqueFilePath(this IFormFile formFile, string fileDestinationDirectory)
        {
            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
            var extension = Path.GetExtension(formFile.FileName);
            var uniqueFileName = $"{fileName}.{DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture)}.{Guid.NewGuid().ToString("N")}{extension}";
            return ($"{fileDestinationDirectory}/{uniqueFileName}",
                Path.Combine(fileDestinationDirectory, uniqueFileName));
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string RemoveIllegalCharacters(this string str)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex regex = new($"[{Regex.Escape(regexSearch)}]");
            str = regex.Replace(str, "");
            return str;
        }
    }
}
