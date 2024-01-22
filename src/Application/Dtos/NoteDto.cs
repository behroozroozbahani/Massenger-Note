using System;
using System.Collections.Generic;

namespace PortalCore.Application.Dtos
{
    public class NoteDto
    {
        public Guid Id { get; set; }
        public string UserFullName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public Guid UserId { get; set; }
        public IEnumerable<NoteFileDto>? NoteFileDtos { get; set; }
    }
}