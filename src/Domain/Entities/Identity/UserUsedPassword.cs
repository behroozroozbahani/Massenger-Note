using System;
using PortalCore.Domain.Common;

namespace PortalCore.Domain.Entities.Identity
{
    public class UserUsedPassword : IEntity, IAuditableEntity
    {
        public Guid Id { get; set; }

        public string HashedPassword { get; set; } = null!;

        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}