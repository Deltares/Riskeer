using System;
using System.ComponentModel;

namespace Core.Common.Utils.Attributes
{
    /// <summary>
    /// Variation on <see cref="DescriptionAttribute"/>, enables the description to be fetched from resources.
    /// </summary>
    /// <remarks>Do not combine this with <see cref="DescriptionAttribute"/> on the same item</remarks>
    public sealed class ResourcesDescriptionAttribute : DescriptionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource file.</param>
        /// <param name="resourceName">Name of the string resource property to be used as description.</param>
        /// <exception cref="InvalidOperationException">Resource cannot be found or does 
        /// not have the given resource name.</exception>
        public ResourcesDescriptionAttribute(Type resourceType, string resourceName) : base(ResourceHelper.GetResourceLookup(resourceType, resourceName)) {}
    }
}