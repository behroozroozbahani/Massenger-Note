using PortalCore.Application;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Infrastructure;
using PortalCore.Infrastructure.DependencyInjectionExtensions;
using PortalCore.Persistence;
using PortalCore.WebApi.Extensions;
using PortalCore.WebApi.Services;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace PortalCore.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomOptions(Configuration);

            services.AddApplication();
            services.AddPersistence(Configuration);
            services.AddInfrastructure(Configuration);

            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            //services.AddScoped<HttpResponseExceptionFilter>();

            services.AddCustomSwagger();
            services.AddCustomCors(Configuration);
            services.AddCustomAntiforgery();
            services.AddCustomHangfire(Configuration);
            services.AddCustomMvc();

            services.AddDNTCommonWeb();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCustomSwagger();

            //app.UseStaticFiles();

            //app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseCustomHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapCustomHangfireDashboard();
            });
        }
    }
}
