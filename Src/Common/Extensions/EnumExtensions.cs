using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace PortalCore.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var result = value
                             .GetType()
                             .GetMember(value.ToString())
                             .FirstOrDefault()
                             ?.GetCustomAttribute<DescriptionAttribute>()
                             ?.Description
                         ?? value.ToString();

            if (result == "0")
                result = string.Empty;

            return result;
        }

        public static string GetDisplayName(this Enum value)
        {
            var result = value
                             .GetType()
                             .GetMember(value.ToString())
                             .FirstOrDefault()
                             ?.GetCustomAttribute<DisplayNameAttribute>()
                             ?.DisplayName
                         ?? value.ToString();

            if (result == "0")
                result = string.Empty;

            return result;
        }

        public static string GetDisplay(this Enum value)
        {
            var result = value
                             .GetType()
                             .GetMember(value.ToString())
                             .FirstOrDefault()
                             ?.GetCustomAttribute<DisplayAttribute>()
                             ?.Name
                         ?? value.ToString();

            if (result == "0")
                result = string.Empty;

            return result;
        }
    }
}
