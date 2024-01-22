using System;
using System.IO;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayFileDto
    {
        public byte[]? Content { get; set; }
        public FileStream? FileContents { get; set; }
        public string? ContentType { get; set; }
        public string? FileDownloadName { get; set; }
    }
}
