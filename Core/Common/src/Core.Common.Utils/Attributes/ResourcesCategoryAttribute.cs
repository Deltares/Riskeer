using System;
using System.ComponentModel;

namespace Core.Common.Utils.Attributes
{
    public class ResourcesCategoryAttribute : CategoryAttribute
    {
        public ResourcesCategoryAttribute(Type resourceType, string resourceName)
            : base(ResourceHelper.GetResourceLookup(resourceType, resourceName)) {}
    }
}