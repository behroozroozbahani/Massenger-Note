using System;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Identity;

namespace PortalCore.Common.Models.SiteSettings
{
    public class SiteSettings
    {
        public Logging Logging { get; set; } = null!;

        public string CorsOrigins { get; set; } = "*";

        public BearerTokensSettings BearerTokensSettings { get; set; } = null!;

        public ApiSettings ApiSettings { get; set; } = null!;

        public AdminUserSeed AdminUserSeed { get; set; } = null!;

        public SmtpConfig? Smtp { get; set; }

        public SmsSettings SmsSettings { get; set; } = null!;

        public int NotAllowedPreviouslyUsedPasswords { get; set; }

        public int ChangePasswordReminderDays { get; set; }

        public bool EnableEmailConfirmation { get; set; }

        public TimeSpan EmailConfirmationTokenProviderLifespan { get; set; } = TimeSpan.FromDays(1);

        public PasswordOptions? PasswordOptions { get; set; }

        public LockoutOptions? LockoutOptions { get; set; }

        public string[] EmailsBanList { get; set; } = null!;

        public string[] PasswordsBanList { get; set; } = null!;
    }
}
