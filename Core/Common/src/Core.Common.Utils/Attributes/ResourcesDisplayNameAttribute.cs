using System;
using System.ComponentModel;

namespace Core.Common.Utils.Attributes
{
    /// <summary>
    /// Variation on <see cref="DisplayNameAttribute"/>, enables the display name to be fetched from resources.
    /// </summary>
    /// <remarks>Do not combine this with <see cref="DisplayNameAttribute"/> on the same item</remarks>
    public sealed class ResourcesDisplayNameAttribute : DisplayNameAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesDisplayNameAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource file.</param>
        /// <param name="resourceName">Name of the string resource property to be used as display name.</param>
        /// <exception cref="InvalidOperationException">Resource cannot be found or does 
        /// not have the given resource name.</exception>
        public ResourcesDisplayNameAttribute(Type resourceType, string resourceName) : base(ResourceHelper.GetResourceLookup(resourceType, resourceName)) {}
    }
}