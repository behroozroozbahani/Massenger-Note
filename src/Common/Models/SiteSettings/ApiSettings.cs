namespace PortalCore.Common.Models.SiteSettings
{
    public class ApiSettings
    {
        public string LoginPath { get; set; } = null!;
        public string LogoutPath { get; set; } = null!;
        public string RefreshTokenPath { get; set; } = null!;
        public string AccessTokenObjectKey { get; set; } = null!;
        public string RefreshTokenObjectKey { get; set; } = null!;
        public string AdminRoleName { get; set; } = null!;
    }
}
