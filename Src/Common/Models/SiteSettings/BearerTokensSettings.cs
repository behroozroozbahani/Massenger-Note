namespace PortalCore.Common.Models.SiteSettings
{
    public class BearerTokensSettings
    {
        public string Key { set; get; } = null!;
        public string Issuer { set; get; } = null!;
        public string Audience { set; get; } = null!;
        public int AccessTokenExpirationMinutes { set; get; }
        public int RefreshTokenExpirationMinutes { set; get; }
        public bool AllowMultipleLoginsFromTheSameUser { set; get; }
        public bool AllowSignoutAllUserActiveClients { set; get; }
    }
}
