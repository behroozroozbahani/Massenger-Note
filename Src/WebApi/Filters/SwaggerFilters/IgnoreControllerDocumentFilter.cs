using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PortalCore.WebApi.Filters.SwaggerFilters
{
    public class IgnoreControllerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathList = swaggerDoc.Paths.Where((x) => x.Key.StartsWith("/api/CspReport", StringComparison.OrdinalIgnoreCase)).ToList();

            // If paths are found, simply remove them from list
            if (pathList is {Count: > 0})
            {
                // Loop through all paths and remove it from our list
                pathList.ForEach(result =>
                {
                    swaggerDoc.Paths.Remove(result.Key);
                });
            }
        }
    }
}
