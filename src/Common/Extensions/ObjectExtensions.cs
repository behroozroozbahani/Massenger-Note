using System;

namespace PortalCore.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }

        //public static bool HasPropertyWithoutPagedQueryModel(this object obj)
        //{
        //    var excludeProperties = typeof(PagedQueryModel).GetProperties();
        //    var excludePropertiesNames = excludeProperties.Select(p => p.Name).ToList();
        //    var properties = obj.GetType().GetProperties();
        //    var propertiesNames = properties.Select(p => p.Name).ToList();

        //    var result = propertiesNames.Except(excludePropertiesNames).Any();
        //    return result;
        //}
    }
}
