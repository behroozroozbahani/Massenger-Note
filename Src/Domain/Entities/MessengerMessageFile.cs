using PortalCore.Domain.Common;
using System;

namespace PortalCore.Domain.Entities
{
    public class MessengerMessageFile : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FileThumbNail { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public string FileMime { get; set; } = null!;
        public string FileRealName { get; set; } = null!;
        public MessengerPrivateMessage? MessengerPrivateMessage { get; set; }
        public Guid? MessengerPrivateMessageId { get; set; }
        public MessengerGroupMessage? MessengerGroupMessage { get; set; }
        public Guid? MessengerGroupMessageId { get; set; }
    }
}
