using PortalCore.Common.Models.SiteSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace PortalCore.Persistence.DependencyInjectionExtensions
{
    public static class IdentityServicesRegistry
    {
        /// <summary>
        /// Adds all of the ASP.NET Core Identity related services and configurations at once.
        /// </summary>
        public static void AddCustomIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            var siteSettings = GetSiteSettings(services);
            services.AddIdentityOptions(siteSettings);
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddCustomInMemoryDbContext();
            }
            else
            {
                services.AddCustomDbContextPool(configuration);
            }
            services.AddIdentityCustomServices();
        }

        public static SiteSettings GetSiteSettings(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var siteSettingsOptions = provider.GetRequiredService<IOptionsSnapshot<SiteSettings>>();
            var siteSettings = siteSettingsOptions.Value;
            if (siteSettings == null) throw new ArgumentNullException(nameof(siteSettings));
            return siteSettings;
        }
    }
}
