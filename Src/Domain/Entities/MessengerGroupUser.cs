using PortalCore.Domain.Common;
using PortalCore.Domain.Entities.Identity;
using System;
using System.Collections.Generic;

namespace PortalCore.Domain.Entities
{
    public class MessengerGroupUser : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public User User { get; set; } = null!;
        public Guid UserId { get; set; }
        public bool IsAdmin { get; set; }
        public MessengerGroup MessengerGroup { get; set; } = null!;
        public Guid MessengerGroupId { get; set; }
    }
}
