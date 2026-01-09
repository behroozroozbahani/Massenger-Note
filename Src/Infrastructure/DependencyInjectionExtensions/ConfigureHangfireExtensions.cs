using PortalCore.Common.Constants;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace PortalCore.Infrastructure.DependencyInjectionExtensions
{
    public static class ConfigureHangfireExtensions
    {
        private class HangfireCustomAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                if (httpContext is null)
                {
                    return false;
                }

                if (httpContext.User.Identity is null)
                {
                    return false;
                }

                if (httpContext.User.Identity.IsAuthenticated)
                {
                    if (httpContext.User.IsInRole(ConstantRoles.Admin) ||
                        httpContext.User.IsInRole(ConstantRoles.Manager))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static void UseCustomHangfireDashboard(this IApplicationBuilder app)
        {
            var dashboardOptions = new DashboardOptions
            {
                Authorization = new[] { new HangfireCustomAuthorizationFilter() },
            };

            // HangFire Dashboard
            app.UseHangfireDashboard("/hangfire", dashboardOptions);
        }

        public static void MapCustomHangfireDashboard(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // HangFire Dashboard endpoint
            endpointRouteBuilder.MapHangfireDashboard();
        }

        public static void AddCustomHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("HangfireConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            // Add Hangfire services.
            services.AddHangfire(globalConfiguration => globalConfiguration
                .UseSerilogLogProvider()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer(options =>
            {
                //options.SchedulePollingInterval = TimeSpan.FromSeconds(10);//default is 15 second!
            });
        }
    }
}
