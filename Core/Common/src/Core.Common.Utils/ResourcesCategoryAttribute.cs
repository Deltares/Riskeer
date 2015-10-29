using System;
using System.ComponentModel;

namespace Core.Common.Utils
{
    public class ResourcesCategoryAttribute : CategoryAttribute
    {
        public ResourcesCategoryAttribute(Type resourceType, string resourceName)
            : base(ResourceHelper.GetResourceLookup(resourceType, resourceName)) {}
    }
}