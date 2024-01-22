using System;
using System.Linq;
using System.Threading.Tasks;
using PortalCore.Application.Common.Identity;
using PortalCore.Common.Constants;
using PortalCore.Common.Extensions;
using PortalCore.Common.IdentityToolkit;
using PortalCore.Common.Models.SiteSettings;
using PortalCore.Domain.Entities.Identity;
using PortalCore.Persistence.Context;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PortalCore.Persistence.Services.Identity
{
    public class IdentityDbInitializer : IIdentityDbInitializer
    {
        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
        private readonly IApplicationUserManager _applicationUserManager;
        private readonly ILogger<IdentityDbInitializer> _logger;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IServiceScopeFactory _scopeFactory;

        public IdentityDbInitializer(
            IApplicationUserManager applicationUserManager,
            IServiceScopeFactory scopeFactory,
            IApplicationRoleManager roleManager,
            IOptionsSnapshot<SiteSettings> adminUserSeedOptions,
            ILogger<IdentityDbInitializer> logger
            )
        {
            _applicationUserManager = applicationUserManager ?? throw new ArgumentNullException(nameof(applicationUserManager));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _adminUserSeedOptions = adminUserSeedOptions ?? throw new ArgumentNullException(nameof(adminUserSeedOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        public void Initialize()
        {
            try
            {
                _scopeFactory.RunScopedService<ApplicationDbContext>(context =>
                {
                    context.Database.Migrate();
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "IdentityDbInitializer Initialize error");
            }
        }

        /// <summary>
        /// Adds some default values to the IdentityDb
        /// </summary>
        public void SeedData()
        {
            try
            {
                _scopeFactory.RunScopedService<IIdentityDbInitializer>(identityDbSeedData =>
                {
                    var result = identityDbSeedData.SeedDatabaseWithAdminUserAsync().Result;
                    if (result == IdentityResult.Failed())
                    {
                        throw new InvalidOperationException(result.DumpErrors());
                    }
                });

                _scopeFactory.RunScopedService<ApplicationDbContext>(context =>
                {
                    if (!context.Roles.Any())
                    {
                        context.Add(new Role(ConstantRoles.Admin));
                        context.Add(new Role(ConstantRoles.User));
                        context.SaveChanges();
                    }
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "IdentityDbInitializer SeedData error");
            }
        }

        public async Task<IdentityResult> SeedDatabaseWithAdminUserAsync()
        {
            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;
            adminUserSeed.CheckArgumentIsNull(nameof(adminUserSeed));

            var name = adminUserSeed.Username;
            var password = adminUserSeed.Password;
            var email = adminUserSeed.Email;
            var roleName = adminUserSeed.RoleName;

            var thisMethodName = nameof(SeedDatabaseWithAdminUserAsync);

            var adminUser = await _applicationUserManager.FindByNameAsync(name);
            if (adminUser != null)
            {
                _logger.LogInformation($"{thisMethodName}: adminUser already exists.");
                return IdentityResult.Success;
            }

            //Create the `Admin` Role if it does not exist
            var adminRole = await _roleManager.FindByNameAsync(roleName);
            if (adminRole == null)
            {
                adminRole = new Role(roleName);
                var adminRoleResult = await _roleManager.CreateAsync(adminRole);
                if (adminRoleResult == IdentityResult.Failed())
                {
                    _logger.LogError($"{thisMethodName}: adminRole CreateAsync failed. {adminRoleResult.DumpErrors()}");
                    return IdentityResult.Failed();
                }
            }
            else
            {
                _logger.LogInformation($"{thisMethodName}: adminRole already exists.");
            }

            adminUser = new User
            {
                UserName = name,
                IsActive = true,
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = true,
                SerialNumber = Guid.NewGuid().ToString("N"),
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var adminUserResult = await _applicationUserManager.CreateAsync(adminUser, password);
            if (adminUserResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser CreateAsync failed. {adminUserResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            var setLockoutResult = await _applicationUserManager.SetLockoutEnabledAsync(adminUser, enabled: false);
            if (setLockoutResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser SetLockoutEnabledAsync failed. {setLockoutResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            var addToRoleResult = await _applicationUserManager.AddToRoleAsync(adminUser, adminRole.Name);
            if (addToRoleResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser AddToRoleAsync failed. {addToRoleResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }
    }
}