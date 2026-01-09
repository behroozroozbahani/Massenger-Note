using System;
using PortalCore.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace PortalCore.Domain.Entities.Identity
{
    public class UserLogin : IdentityUserLogin<Guid>, IBaseEntity, IAuditableEntity
    {
        public virtual User User { get; set; } = null!;
    }
}