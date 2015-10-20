using System;
using System.ComponentModel;
using System.Linq;

namespace DelftTools.Utils.Reflection
{
    public static class TypeExtensions
    {
        public static string GetDisplayName(this Type type)
        {
            var displayNameAttribute = type.GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault();

            return displayNameAttribute != null ? displayNameAttribute.DisplayName : type.Name;
        }
    }
}