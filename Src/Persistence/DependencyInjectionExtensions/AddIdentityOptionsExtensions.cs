using System;
using System.Threading.Tasks;
using PortalCore.Common.Models.SiteSettings;
using PortalCore.Domain.Entities.Identity;
using PortalCore.Persistence.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PortalCore.Persistence.DependencyInjectionExtensions
{
    public static class AddIdentityOptionsExtensions
    {
        public const string EmailConfirmationTokenProviderName = "ConfirmEmail";

        public static IServiceCollection AddIdentityOptions(
            this IServiceCollection services, SiteSettings siteSettings)
        {
            if (siteSettings == null) throw new ArgumentNullException(nameof(siteSettings));

            services.addConfirmEmailDataProtectorTokenOptions(siteSettings);
            services.AddIdentity<User, Role>(identityOptions =>
            {
                setPasswordOptions(identityOptions.Password, siteSettings);
                setSignInOptions(identityOptions.SignIn, siteSettings);
                setUserOptions(identityOptions.User);
                setLockoutOptions(identityOptions.Lockout, siteSettings);
            }).AddUserStore<ApplicationUserStore>()
              .AddUserManager<ApplicationUserManager>()
              .AddRoleStore<ApplicationRoleStore>()
              .AddRoleManager<ApplicationRoleManager>()
              .AddSignInManager<ApplicationSignInManager>()
              .AddErrorDescriber<CustomIdentityErrorDescriber>()
              // You **cannot** use .AddEntityFrameworkStores() when you customize everything
              //.AddEntityFrameworkStores<ApplicationDbContext, int>()
              .AddDefaultTokenProviders()
              .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<User>>(EmailConfirmationTokenProviderName);

            //services.ConfigureApplicationCookie(identityOptionsCookies =>
            //{
            //    var provider = services.BuildServiceProvider();
            //    setApplicationCookieOptions(provider, identityOptionsCookies, siteSettings);
            //});

            services.enableImmediateLogout();

            return services;
        }

        private static void addConfirmEmailDataProtectorTokenOptions(this IServiceCollection services, SiteSettings siteSettings)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProviderName;
            });

            services.Configure<ConfirmEmailDataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = siteSettings.EmailConfirmationTokenProviderLifespan;
            });
        }

        private static void enableImmediateLogout(this IServiceCollection services)
        {
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                options.ValidationInterval = TimeSpan.Zero;
                options.OnRefreshingPrincipal = principalContext =>
                {
                    // Invoked when the default security stamp validator replaces the user's ClaimsPrincipal in the cookie.

                    //var newId = new ClaimsIdentity();
                    //newId.AddClaim(new Claim("PreviousName", principalContext.CurrentPrincipal.Identity.Name));
                    //principalContext.NewPrincipal.AddIdentity(newId);

                    return Task.CompletedTask;
                };
            });
        }

        //private static void setApplicationCookieOptions(IServiceProvider provider, CookieAuthenticationOptions identityOptionsCookies, SiteSettings siteSettings)
        //{
        //    identityOptionsCookies.Cookie.Name = siteSettings.CookieOptions.CookieName;
        //    identityOptionsCookies.Cookie.HttpOnly = true;
        //    identityOptionsCookies.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        //    identityOptionsCookies.Cookie.SameSite = SameSiteMode.Lax;
        //    identityOptionsCookies.Cookie.IsEssential = true; //  this cookie will always be stored regardless of the user's consent

        //    identityOptionsCookies.ExpireTimeSpan = siteSettings.CookieOptions.ExpireTimeSpan;
        //    identityOptionsCookies.SlidingExpiration = siteSettings.CookieOptions.SlidingExpiration;
        //    identityOptionsCookies.LoginPath = siteSettings.CookieOptions.LoginPath;
        //    identityOptionsCookies.LogoutPath = siteSettings.CookieOptions.LogoutPath;
        //    identityOptionsCookies.AccessDeniedPath = siteSettings.CookieOptions.AccessDeniedPath;

        //    if (siteSettings.CookieOptions.UseDistributedCacheTicketStore)
        //    {
        //        // To manage large identity cookies
        //        identityOptionsCookies.SessionStore = provider.GetRequiredService<ITicketStore>();
        //    }
        //}

        private static void setLockoutOptions(LockoutOptions identityOptionsLockout, SiteSettings siteSettings)
        {
            if (siteSettings.LockoutOptions is null) return;

            identityOptionsLockout.AllowedForNewUsers = siteSettings.LockoutOptions.AllowedForNewUsers;
            identityOptionsLockout.DefaultLockoutTimeSpan = siteSettings.LockoutOptions.DefaultLockoutTimeSpan;
            identityOptionsLockout.MaxFailedAccessAttempts = siteSettings.LockoutOptions.MaxFailedAccessAttempts;
        }

        private static void setPasswordOptions(PasswordOptions identityOptionsPassword, SiteSettings siteSettings)
        {
            if (siteSettings.PasswordOptions is null) return;

            identityOptionsPassword.RequireDigit = siteSettings.PasswordOptions.RequireDigit;
            identityOptionsPassword.RequireLowercase = siteSettings.PasswordOptions.RequireLowercase;
            identityOptionsPassword.RequireNonAlphanumeric = siteSettings.PasswordOptions.RequireNonAlphanumeric;
            identityOptionsPassword.RequireUppercase = siteSettings.PasswordOptions.RequireUppercase;
            identityOptionsPassword.RequiredLength = siteSettings.PasswordOptions.RequiredLength;
        }

        private static void setSignInOptions(SignInOptions identityOptionsSignIn, SiteSettings siteSettings)
        {
            if (siteSettings.PasswordOptions is null) return;

            identityOptionsSignIn.RequireConfirmedEmail = siteSettings.EnableEmailConfirmation;
            identityOptionsSignIn.RequireConfirmedAccount = false;
        }

        private static void setUserOptions(UserOptions identityOptionsUser)
        {
            identityOptionsUser.RequireUniqueEmail = false;
        }
    }
}
