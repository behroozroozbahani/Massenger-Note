using PortalCore.Domain.Common;
using PortalCore.Domain.Entities.Identity;
using System;
using System.Collections.Generic;

namespace PortalCore.Domain.Entities
{
    public class MessengerGroup : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; } = true;
        public User Owner { get; set; } = null!;
        public Guid OwnerId { get; set; }
        public virtual ICollection<MessengerGroupUser> MessengerGroupUsers { get; set; }
        public virtual ICollection<MessengerGroupMessage> MessengerGroupMessages  { get; set; }

        public MessengerGroup()
        {
            MessengerGroupUsers = new HashSet<MessengerGroupUser>();
            MessengerGroupMessages = new HashSet<MessengerGroupMessage>();
        }
    }
}
