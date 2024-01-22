using System;

namespace PortalCore.Application.Common.Token
{
    public class JwtUserInfo
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string DisplayName
        {
            get
            {
                var displayName = $"{FirstName} {LastName}";
                return string.IsNullOrWhiteSpace(displayName) ? UserName : displayName;
            }
        }

        public string SerialNumber { get; set; } = null!;
    }
}
