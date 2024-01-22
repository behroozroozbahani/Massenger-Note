using System;
using System.Threading.Tasks;
using PortalCore.Application.Common.Identity;
using PortalCore.Common.Models.SiteSettings;
using DNTCommon.Web.Core;
using Microsoft.Extensions.Options;

namespace PortalCore.Persistence.Services.Identity
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IOptionsSnapshot<SiteSettings> _smtpConfig;
        private readonly IWebMailService _webMailService;

        public AuthMessageSender(
            IOptionsSnapshot<SiteSettings> smtpConfig,
            IWebMailService webMailService)
        {
            _smtpConfig = smtpConfig ?? throw new ArgumentNullException(nameof(smtpConfig));
            _webMailService = webMailService ?? throw new ArgumentNullException(nameof(webMailService));
        }

        public Task SendEmailAsync<T>(string email, string subject, string viewNameOrPath, T model)
        {
            var smtp = _smtpConfig.Value.Smtp;
            if (smtp is null) return Task.CompletedTask;

            return _webMailService.SendEmailAsync(
                smtp,
                new[] { new MailAddress { ToName = "", ToAddress = email } },
                subject,
                viewNameOrPath,
                model
            );
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var smtp = _smtpConfig.Value.Smtp;
            if (smtp is null) return Task.CompletedTask;

            return _webMailService.SendEmailAsync(
                smtp,
                new[] { new MailAddress { ToName = "", ToAddress = email } },
                subject,
                message
            );
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}