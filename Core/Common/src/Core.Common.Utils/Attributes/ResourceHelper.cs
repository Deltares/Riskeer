using System;
using System.Reflection;

namespace Core.Common.Utils.Attributes
{
    /// <summary>
    /// Helper method for interacting with project resources.
    /// </summary>
    internal static class ResourceHelper
    {
        /// <summary>
        /// Retrieve the string resource with the given name.
        /// </summary>
        /// <param name="resourceType">Type of the resource file.</param>
        /// <param name="resourceName">Name of the resource property to be retrieved.</param>
        /// <returns>String resource in the resources file.</returns>
        /// <exception cref="InvalidOperationException">Resource cannot be found or does
        /// not have the given resource name or isn't of type string.</exception>
        internal static string GetResourceLookup(Type resourceType, string resourceName)
        {
            var property = resourceType.GetProperty(resourceName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (property == null)
            {
                var message = string.Format("Resource {0} does not have property {1}.",
                                            resourceType, resourceName);
                throw new InvalidOperationException(message);
            }
            if (property.PropertyType != typeof(string))
            {
                var message = string.Format("Resource {0} is not string.",
                                            resourceName);
                throw new InvalidOperationException(message);
            }
            return (string) property.GetValue(null, null);
        }
    }
}