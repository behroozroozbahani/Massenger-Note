using Microsoft.AspNetCore.Identity;
using PortalCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalCore.Domain.Entities.Identity
{
    public class User : IdentityUser<Guid>, IEntity, IAuditableEntity
    {
        public User()
        {
            UserUsedPasswords = new HashSet<UserUsedPassword>();
            UserTokens = new HashSet<UserToken>();
            Roles = new HashSet<UserRole>();
            Logins = new HashSet<UserLogin>();
            Claims = new HashSet<UserClaim>();
            JwtUserTokens = new HashSet<JwtUserToken>();
            Notes = new List<Note>();
            MessengerGroups = new List<MessengerGroup>();
            MessengerGroupUsers = new List<MessengerGroupUser>();
            SentMessages = new List<MessengerPrivateMessage>();
            RecipientMessages = new List<MessengerPrivateMessage>();
            MessengerGroupMessages = new List<MessengerGroupMessage>();
            MessengerGroupMessageSeenMessages = new List<MessengerGroupMessageSeenMessage>();
        }

        [StringLength(450)]
        public string? FirstName { get; set; }

        [StringLength(450)]
        public string? LastName { get; set; }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                var displayName = $"{FirstName} {LastName}";
                return string.IsNullOrWhiteSpace(displayName) ? UserName : displayName;
            }
        }

        public string? ProfileImage { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastVisitDateTime { get; set; }

        public DateTimeOffset? LastLoggedIn { get; set; }

        /// <summary>
        /// every time the user changes his Password,
        /// or an admin changes his Roles or stat/IsActive,
        /// create a new `SerialNumber` GUID and store it in the DB.
        /// </summary>
        public string SerialNumber { get; set; } = null!;

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string? MobileNumber { get; set; }

        public virtual ICollection<UserUsedPassword> UserUsedPasswords { get; set; }

        public virtual ICollection<UserToken> UserTokens { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }

        public virtual ICollection<UserLogin> Logins { get; set; }

        public virtual ICollection<UserClaim> Claims { get; set; }

        public virtual ICollection<JwtUserToken> JwtUserTokens { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<MessengerGroup> MessengerGroups { get; set; }
        public virtual ICollection<MessengerGroupUser> MessengerGroupUsers { get; set; }
        public virtual ICollection<MessengerPrivateMessage> SentMessages { get; set; }
        public virtual ICollection<MessengerPrivateMessage> RecipientMessages { get; set; }
        public virtual ICollection<MessengerGroupMessage> MessengerGroupMessages { get; set; }
        public virtual ICollection<MessengerGroupMessageSeenMessage> MessengerGroupMessageSeenMessages{ get; set; }
    }
}