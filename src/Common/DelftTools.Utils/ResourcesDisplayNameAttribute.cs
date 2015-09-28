using System;
using System.ComponentModel;

namespace DelftTools.Utils
{
    /// <summary>
    /// Variation on <see cref="DisplayNameAttribute"/>, enables the display name to be fetched from resources.
    /// </summary>
    /// <remarks>Do not combine this with <see cref="DisplayNameAttribute"/> on the same item</remarks>
    public class ResourcesDisplayNameAttribute : DisplayNameAttribute
    {
        public ResourcesDisplayNameAttribute(Type resourceType, string resourceName)
            : base(ResourceHelper.GetResourceLookup(resourceType, resourceName))
        {
        }
    }
}