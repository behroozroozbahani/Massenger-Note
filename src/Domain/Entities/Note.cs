using PortalCore.Domain.Common;
using PortalCore.Domain.Entities.Identity;
using System;
using System.Collections.Generic;

namespace PortalCore.Domain.Entities
{
    public class Note : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public User User { get; set; } = null!;
        public Guid UserId { get; set; }
        public ICollection<NoteFile> NoteFiles { get; set; }

        public Note()
        {
            NoteFiles = new List<NoteFile>();
        }
    }
}
