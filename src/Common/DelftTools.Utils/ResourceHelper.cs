using System;
using System.Reflection;
using DelftTools.Utils.Properties;

namespace DelftTools.Utils
{
    /// <summary>
    /// Helper method for interacting with <see cref="Resource"/>.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Retrieve the string resource with the given name.
        /// </summary>
        /// <param name="resourceType">Type of the resource file.</param>
        /// <param name="resourceName">Name of the resource property to be retrieved.</param>
        /// <returns>String resource in the resources file.</returns>
        public static string GetResourceLookup(Type resourceType, string resourceName)
        {
            if ((resourceType != null) && (resourceName != null))
            {
                var property = resourceType.GetProperty(resourceName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                {
                    throw new InvalidOperationException(string.Format("Resource Type Does Not Have Property"));
                }
                if (property.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException(string.Format("Resource Property is Not String Type"));
                }
                return (string)property.GetValue(null, null);
            }
            return null;
        }
    }
}