using System;
using System.ComponentModel;

namespace Core.Common.Utils.Attributes
{
    /// <summary>
    /// Attribute that allows for defining <see cref="CategoryAttribute"/> using textual resources.
    /// </summary>
    /// <remarks>Do not combine with <see cref="CategoryAttribute"/>.</remarks>
    public sealed class ResourcesCategoryAttribute : CategoryAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesCategoryAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource file.</param>
        /// <param name="resourceName">Name of the string resource property to be used as category.</param>
        /// <exception cref="InvalidOperationException">Resource cannot be found or does 
        /// not have the given resource name.</exception>
        public ResourcesCategoryAttribute(Type resourceType, string resourceName) : base(ResourceHelper.GetResourceLookup(resourceType, resourceName)) {}
    }
}