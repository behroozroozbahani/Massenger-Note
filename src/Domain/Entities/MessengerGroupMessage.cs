using PortalCore.Domain.Common;
using PortalCore.Domain.Entities.Identity;
using System;
using System.Collections.Generic;

namespace PortalCore.Domain.Entities
{
    public class MessengerGroupMessage : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public User Sender { get; set; } = null!;
        public Guid SenderId { get; set; }
        public string MessageBody { get; set; } = null!;
        public DateTimeOffset SendDateTime { get; set; }
        public MessengerGroup MessengerGroup { get; set; } = null!;
        public Guid MessengerGroupId { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsAdmin { get; set; }
        public MessengerGroupMessage? ParentMessage { get; set; }
        public virtual ICollection<MessengerMessageFile> MessengerMessageFiles { get; set; }
        public virtual ICollection<MessengerGroupMessageSeenMessage> MessengerGroupMessageSeenMessages { get; set; }
        public virtual ICollection<MessengerGroupMessage> MessengerGroupMessages { get; set; }

        public MessengerGroupMessage()
        {
            MessengerMessageFiles = new HashSet<MessengerMessageFile>();
            MessengerGroupMessageSeenMessages = new HashSet<MessengerGroupMessageSeenMessage>();
            MessengerGroupMessages = new HashSet<MessengerGroupMessage>();
        }
    }
}
