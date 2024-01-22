using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayMessagesDto
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; } = null!;
        public string SendDate { get; set; } = null!;
        public Guid? ParentId { get; set; }
        public string? ParentMessage { get; set; }
        public bool IsRead { get; set; }
        public FileStream? FileContents { get; set; }
        public string? ContentType { get; set; }
        public string? FileDownloadName { get; set; }
        public List<FileStreamResult> FileStreamResults { get; set; } = null!;

        //public List<MessengerDisplayMessageFilesDto> MessengerDisplayMessageFilesDtos { get; set; } = null!;
    }
}
