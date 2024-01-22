using System;
using PortalCore.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace PortalCore.Domain.Entities.Identity
{
    public class UserRole : IdentityUserRole<Guid>, IBaseEntity, IAuditableEntity
    {
        public virtual User User { get; set; } = null!;

        public virtual Role Role { get; set; } = null!;
    }
}