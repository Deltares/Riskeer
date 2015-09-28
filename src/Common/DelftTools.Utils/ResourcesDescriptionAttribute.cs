using System;
using System.ComponentModel;

namespace DelftTools.Utils
{
    /// <summary>
    /// Variation on <see cref="DescriptionAttribute"/>, enables the description to be fetched from resources.
    /// </summary>
    /// <remarks>Do not combine this with <see cref="DescriptionAttribute"/> on the same item</remarks>
    public class ResourcesDescriptionAttribute : DescriptionAttribute
    {
        public ResourcesDescriptionAttribute(Type resourceType, string resourceName) : base(ResourceHelper.GetResourceLookup(resourceType, resourceName))
        {
        }
    }
}