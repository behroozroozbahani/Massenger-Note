using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Infrastructure.Files;
using PortalCore.Infrastructure.Services;
using PortalCore.Infrastructure.Services.GenerateFileName;
using PortalCore.Infrastructure.Services.MessengerCheckImages;
using PortalCore.Infrastructure.Services.Thumbnail;

namespace PortalCore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDomainEventService, DomainEventService>();

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

            services.AddSingleton<ICustomUploadFileService, CustomUploadFileService>();

            services.AddTransient<IMessengerCheckImagesService, MessengerCheckImagesService>();
            services.AddTransient<IThumbnailService, ThumbnailService>();
            services.AddTransient<IGenerateFileNameService, GenerateFileNameService>();

            return services;
        }
    }
}