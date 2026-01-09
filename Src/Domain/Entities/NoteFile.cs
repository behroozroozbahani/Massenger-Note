using PortalCore.Domain.Common;
using System;

namespace PortalCore.Domain.Entities
{
    public class NoteFile : IEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public Note Note { get; set; } = null!;
        public Guid NoteId { get; set; }
    }
}
