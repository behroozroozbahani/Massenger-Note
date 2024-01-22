using PortalCore.Domain.Entities.Identity;
using System;

namespace PortalCore.Domain.Entities
{
    public class MessengerGroupMessageSeenMessage
    {
        public MessengerGroupMessage MessengerGroupMessage { get; set; } = null!;
        public Guid MessengerGroupMessageId { get; set; }
        public User User { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
