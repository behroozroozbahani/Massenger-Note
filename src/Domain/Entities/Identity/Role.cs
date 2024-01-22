using System;
using System.Collections.Generic;
using PortalCore.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace PortalCore.Domain.Entities.Identity
{
    public class Role : IdentityRole<Guid>, IEntity, IAuditableEntity
    {
        public Role()
        {
        }

        public Role(string name)
            : base(name)
        {
            
        }

        public Role(string name, string description)
            : base(name)
        {
            Description = description;
        }

        public string? Description { get; set; }

        public virtual ICollection<UserRole>? Users { get; set; }

        public virtual ICollection<RoleClaim>? Claims { get; set; }
    }
}