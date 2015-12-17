﻿using System;
using System.Reflection;

using Core.Common.Utils.Properties;

namespace Core.Common.Utils.Attributes
{
    /// <summary>
    /// Helper method for interacting with <see cref="Resource"/>.
    /// </summary>
    internal static class ResourceHelper
    {
        /// <summary>
        /// Retrieve the string resource with the given name.
        /// </summary>
        /// <param name="resourceType">Type of the resource file.</param>
        /// <param name="resourceName">Name of the resource property to be retrieved.</param>
        /// <returns>String resource in the resources file.</returns>
        internal static string GetResourceLookup(Type resourceType, string resourceName)
        {
            if ((resourceType != null) && (resourceName != null))
            {
                var property = resourceType.GetProperty(resourceName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                {
                    throw new InvalidOperationException("Resource Type Does Not Have Property");
                }
                if (property.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException("Resource Property is Not String Type");
                }
                return (string) property.GetValue(null, null);
            }
            return null;
        }
    }
}