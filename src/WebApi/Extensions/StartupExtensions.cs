using System;
using System.IO;
using System.Linq;
using System.Reflection;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Common.Extensions;
using PortalCore.Common.Models.SiteSettings;
using PortalCore.WebApi.Filters;
using PortalCore.WebApi.Filters.SwaggerFilters;
using DNTCommon.Web.Core;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace PortalCore.WebApi.Extensions
{
    public static class StartupExtensions
    {
        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SiteSettings>(options => configuration.Bind(options));

            services.AddOptions<BearerTokensSettings>()
                .Bind(configuration.GetSection("BearerTokensSettings"))
                .Validate(bearerTokens =>
                {
                    return bearerTokens.AccessTokenExpirationMinutes < bearerTokens.RefreshTokenExpirationMinutes;
                }, "RefreshTokenExpirationMinutes is less than AccessTokenExpirationMinutes. Obtaining new tokens using the refresh token should happen only if the access token has expired.");

            services.AddOptions<ApiSettings>()
                .Bind(configuration.GetSection("ApiSettings"));

            //services.AddOptions<PayamakSettings>()
            //    .Bind(configuration.GetSection("PayamakSettings"));
        }

        public static void AddCustomAntiforgery(this IServiceCollection services)
        {
            services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");
        }

        public static void AddCustomMvc(this IServiceCollection services)
        {
            services.AddControllers(options =>
                {
                    options.UseYeKeModelBinder();

                    //از حالت‌های امنی مانند GET و HEAD صرفنظر می‌کند
                    //به تمام اکشن متدهای HttpPost برنامه به صورت خودکار اعمال میشود
                    //AutoValidateAntiforgeryTokenAttribute allows to apply Anti-forgery token validation globally to all unsafe methods e.g. POST, PUT, PATCH and DELETE. Thus you don't need to add [ValidateAntiForgeryToken] attribute to each and every action that requires it.
                    //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                    //options.Filters.Add(new AuthorizeFilter());
                    options.Filters.Add(typeof(DynamicAuthorizeFilter));

                    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());

                    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                    options.Filters.Add(new ProducesDefaultResponseTypeAttribute());
                    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                    options.ReturnHttpNotAcceptable = true; // Status406NotAcceptable

                    // remove formatter that turns nulls into 204 - No Content responses
                    // this formatter breaks SPA's Http response JSON parsing
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                    options.OutputFormatters.Insert(0, new HttpNoContentOutputFormatter
                    {
                        TreatNullValueAsNoContent = false
                    });

                    //options.Filters.Add(typeof(HttpResponseExceptionFilter));
                    options.Filters.Add<ApiExceptionFilterAttribute>();
                })
                .AddFluentValidation();

            // Customise default API behaviour
            // override modelstate
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public static void AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SupportNonNullableReferenceTypes();
                setupAction.DocumentFilter<IgnoreControllerDocumentFilter>();
                setupAction.SchemaFilter<RequiredButNullableSchemaFilter>();
                setupAction.SchemaFilter<RequiredNotNullableSchemaFilter>();

                setupAction.SwaggerDoc(
                   name: "LibraryOpenAPISpecification",
                   info: new OpenApiInfo()
                   {
                       Title = "Library API",
                       Version = "1",
                       Description = "Through this API you can access the site's capabilities.",
                       Contact = new OpenApiContact()
                       {
                           Email = "name@site.com",
                           Name = "PortalCore",
                       },
                       License = new OpenApiLicense()
                       {
                           Name = "MIT License",
                           Url = new Uri("https://opensource.org/licenses/MIT")
                       }
                   });

                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                xmlFiles.ForEach(xmlFile => setupAction.IncludeXmlComments(xmlFile));

                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });

                setupAction.OperationFilter<AuthorizationOperationFilter>();

                setupAction.CustomSchemaIds(x => x.FullName);//Use fully qualified object names
                setupAction.SchemaFilter<NamespaceSchemaFilter>();//Makes the namespaces hidden for the schemas
            });
        }

        public static void UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    url: "/swagger/LibraryOpenAPISpecification/swagger.json",
                    name: "Library API");
                //setupAction.RoutePrefix = ""; //--> To be able to access it from this URL: https://localhost:5001/swagger/index.html

                setupAction.DefaultModelExpandDepth(2);
                setupAction.DefaultModelRendering(ModelRendering.Model);
                setupAction.DocExpansion(DocExpansion.None);
                setupAction.EnableDeepLinking();
                setupAction.DisplayOperationId();
            });
        }

        public static void AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOrigins = configuration["CorsOrigins"];

            const string allowedCorsPolicyName = "CorsPolicy";

            if (string.IsNullOrWhiteSpace(corsOrigins) || corsOrigins == "*")
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(allowedCorsPolicyName,
                        builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
                });
            }
            else
            {
                var origins = corsOrigins
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.RemoveFromEnd("/"))
                    .ToArray();

                services.AddCors(options =>
                {
                    options.AddPolicy(allowedCorsPolicyName,
                        builder => builder
                            .WithOrigins(origins) //Note:  The URL must be specified without a trailing slash (/).
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed((host) => true)
                            .AllowCredentials());
                });
            }
        }
    }
}
