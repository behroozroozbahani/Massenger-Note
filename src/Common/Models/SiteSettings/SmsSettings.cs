namespace PortalCore.Common.Models.SiteSettings
{
    public class SmsSettings
    {
        public int SmsCodeLenght { get; set; } = 4;
        public int SmsBlockedMinutes { get; set; } = 3;
        public int SmsExpireMinutes { get; set; } = 480;
    }
}
