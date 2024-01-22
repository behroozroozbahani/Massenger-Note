using System;

namespace PortalCore.Application.Dtos
{
    public class NoteFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
    }
}
