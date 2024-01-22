using PortalCore.Application.Common.Identity;
using PortalCore.Persistence.Context;
using PortalCore.Persistence.Toolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace PortalCore.Persistence.DependencyInjectionExtensions
{
    public static class ConfigureDbContextExtensions
    {
        /// <summary>
        /// Creates and seeds the database.
        /// </summary>
        public static void InitializeDb(this IServiceProvider serviceProvider)
        {
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetService<IIdentityDbInitializer>();
                if (dbInitializer is not null)
                {
                    dbInitializer.Initialize();
                    dbInitializer.SeedData();
                }
            }
        }

        public static void AddCustomDbContextPool(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddEntityFrameworkSqlServer(); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
            services.AddDbContextPool<ApplicationDbContext>(
                (serviceProvider, options) =>
                {
                    options.UseSqlServer(connectionString,
                            sqlServerOptionsAction =>
                            {
                                var minutes = (int)TimeSpan.FromMinutes(3).TotalSeconds;
                                sqlServerOptionsAction.CommandTimeout(minutes);
                                sqlServerOptionsAction.EnableRetryOnFailure();//Enable if TransactionScope disabled
                                sqlServerOptionsAction.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                            })
                        .EnableSensitiveDataLogging();

                    options.UseInternalServiceProvider(serviceProvider); // It's added to access services from the dbcontext, remove it if you are using the normal `AddDbContext` and normal constructor dependency injection.
                    options.AddInterceptors(new PersianYeKeCommandInterceptor());
                    //options.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
                    options.ConfigureWarnings(warnings =>
                    {
                        // ...
                        //Throw if unused Include in queries!
                        //warnings.Throw(CoreEventId.IncludeIgnoredWarning)

                        //
                        //warnings.Log(RelationalEventId.QueryClientEvaluationWarning);
                    });
                });
        }

        public static void AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString,
                        serverDbContextOptionsBuilder =>
                        {
                            var minutes = (int)TimeSpan.FromMinutes(3).TotalSeconds;
                            serverDbContextOptionsBuilder.CommandTimeout(minutes);
                            serverDbContextOptionsBuilder.EnableRetryOnFailure();//Enable if TransactionScope disabled
                            serverDbContextOptionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        })
                    .EnableSensitiveDataLogging();

                options.AddInterceptors(new PersianYeKeCommandInterceptor());
                //options.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
                options.ConfigureWarnings(warnings =>
                {
                    // ...
                    //Throw if unused Include in queries!
                    //warnings.Throw(CoreEventId.IncludeIgnoredWarning)

                    //
                    //warnings.Log(RelationalEventId.QueryClientEvaluationWarning);
                });
            });
        }

        public static void AddCustomInMemoryDbContext(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("PortalCoreDb"));
        }
    }
}
