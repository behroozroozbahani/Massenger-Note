using System;
using System.Text;
using System.Threading.Tasks;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Common.Token;
using PortalCore.Common.Constants;
using PortalCore.Persistence.DependencyInjectionExtensions;
using PortalCore.Persistence.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PortalCore.Persistence.Services.MessengerGroupMessage;
using PortalCore.Persistence.Services.MessengerGroupMessageAdmin;
using PortalCore.Persistence.Services.MessengerPrivateMessage;
using PortalCore.Persistence.Services.MessengerPrivateMessageAdmin;
using PortalCore.Persistence.Services;
using PortalCore.Persistence.Services.MessengerAllUsers;

namespace PortalCore.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCustomIdentityServices(configuration);

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IMessengerPrivateMessageService, MessengerPrivateMessageService>();
            services.AddTransient<IMessengerPrivateMessageAdminService, MessengerPrivateMessageAdminService>();
            services.AddTransient<IMessengerGroupMessageService, MessengerGroupMessageService>();
            services.AddTransient<IMessengerGroupMessageAdminService, MessengerGroupMessageAdminService>();
            services.AddTransient<IMessengerAllUsersService, MessengerAllUsersService>();
            //services.AddTransient<IMessengerMessageFileService, MessengerMessageFileService>();

            services.AddScoped<IAccountService, AccountService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(ConstantRoles.Admin, policy => policy.RequireRole(ConstantRoles.Admin));
                options.AddPolicy(ConstantRoles.Manager, policy => policy.RequireRole(ConstantRoles.Manager));
                options.AddPolicy(ConstantRoles.Customer, policy => policy.RequireRole(ConstantRoles.Customer));
                options.AddPolicy("CanPurge", policy => policy.RequireRole(ConstantRoles.Admin));
            });

            // Needed for jwt auth.
            services
                .AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = configuration["BearerTokensSettings:Issuer"], // site that makes the token
                        ValidateIssuer = false, // TODO: change this to avoid forwarding attacks
                        ValidAudience = configuration["BearerTokensSettings:Audience"], // site that consumes the token
                        ValidateAudience = false, // TODO: change this to avoid forwarding attacks
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["BearerTokensSettings:Key"])),
                        ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                        ValidateLifetime = true, // validate the expiration
                        ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                    };
                    cfg.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger(nameof(JwtBearerEvents));
                            logger.LogError("Authentication failed.", context.Exception);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var tokenValidatorService = context.HttpContext.RequestServices
                                .GetRequiredService<ITokenValidatorService>();
                            return tokenValidatorService.ValidateAsync(context);
                        },
                        OnMessageReceived = context =>
                        {
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger(nameof(JwtBearerEvents));
                            logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}