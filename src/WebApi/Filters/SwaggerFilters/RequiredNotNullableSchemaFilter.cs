﻿using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PortalCore.WebApi.Filters.SwaggerFilters
{
    public class RequiredNotNullableSchemaFilter: ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
            {
                return;
            }

            var notNullableProperties = schema
                .Properties
                .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key))
                .ToList();

            foreach (var property in notNullableProperties)
            {
                schema.Required.Add(property.Key);
            }
        }
    }
}
