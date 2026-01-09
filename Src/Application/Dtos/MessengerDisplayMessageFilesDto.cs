using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayMessageFilesDto
    {
        public byte[]? Content { get; set; }
        public FileStream? FileContents { get; set; }
        public string? ContentType { get; set; }
        public string? FileDownloadName { get; set; }
        public FileStreamResult FileStreamResult { get; set; } = null!;
    }
}
