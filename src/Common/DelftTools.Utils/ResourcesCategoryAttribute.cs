using System;
using System.ComponentModel;

namespace DelftTools.Utils
{
    public class ResourcesCategoryAttribute : CategoryAttribute
    {
        public ResourcesCategoryAttribute(Type resourceType, string resourceName)
            : base(ResourceHelper.GetResourceLookup(resourceType, resourceName))
        {
        }
    }
}