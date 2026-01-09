using PortalCore.Domain.Common;
using PortalCore.Domain.Entities.Identity;
using System;
using System.Collections.Generic;

namespace PortalCore.Domain.Entities
{
    public class MessengerPrivateMessage : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; } = null!;
        public DateTimeOffset SendDateTime { get; set; }
        public bool IsRead { get; set; }
        public Guid SenderRecipientId { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public Guid? ParentId { get; set; }
        public User Sender { get; set; } = null!;
        public User Recipient { get; set; } = null!;
        public MessengerPrivateMessage? ParentMessage { get; set; }
        public virtual ICollection<MessengerMessageFile> MessengerMessageFiles { get; set; }
        public virtual ICollection<MessengerPrivateMessage> MessengerPrivateMessages { get; set; }

        public MessengerPrivateMessage()
        {
            MessengerMessageFiles = new HashSet<MessengerMessageFile>();
            MessengerPrivateMessages = new HashSet<MessengerPrivateMessage>();
        }
    }
}