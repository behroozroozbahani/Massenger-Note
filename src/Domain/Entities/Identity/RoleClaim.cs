using System;
using PortalCore.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace PortalCore.Domain.Entities.Identity
{
    public class RoleClaim : IdentityRoleClaim<Guid>, IBaseEntity, IAuditableEntity
    {
        public virtual Role Role { get; set; } = null!;
    }
}