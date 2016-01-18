using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Properties;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Reflection;

namespace Core.Common.Gui.Forms.PropertyGridView
{
    /// <summary>
    /// Helper class for resolving object properties.
    /// </summary>
    public class PropertyResolver : IPropertyResolver
    {
        private readonly IEnumerable<PropertyInfo> propertyInfos;

        /// <summary>
        /// Creates a new instance of <see cref="PropertyResolver"/> with the given <paramref name="propertyInfos"/>.
        /// </summary>
        /// <param name="propertyInfos">The list of property information objects to obtain the object properties from.</param>
        public PropertyResolver(IEnumerable<PropertyInfo> propertyInfos)
        {
            if (propertyInfos == null)
            {
                throw new ArgumentNullException("propertyInfos", Resources.PropertyResolver_PropertyResolver_Cannot_create_PropertyResolver_without_list_of_PropertyInfo);
            }
            this.propertyInfos = propertyInfos.ToArray();
        }

        /// <summary>
        /// Returns object properties based on the provided <paramref name="sourceData"/>.
        /// </summary>
        /// <param name="sourceData">The source data to get the object properties for.</param>
        /// <returns>An object properties object, or null when no relevant properties object is found.</returns>
        public object GetObjectProperties(object sourceData)
        {
            if (sourceData == null)
            {
                return null;
            }

            // 1. Match property information based on ObjectType and on AdditionalDataCheck
            var filteredPropertyInfos = propertyInfos.Where(pi => pi.ObjectType.IsInstanceOfType(sourceData) && (pi.AdditionalDataCheck == null || pi.AdditionalDataCheck(sourceData))).ToArray();

            // 2. Match property information based on object type inheritance
            filteredPropertyInfos = FilterPropertyInfoByTypeInheritance(filteredPropertyInfos, pi => pi.ObjectType);

            // 3. Match property information based on property type inheritance
            filteredPropertyInfos = FilterPropertyInfoByTypeInheritance(filteredPropertyInfos, pi => pi.PropertyType);

            if (filteredPropertyInfos.Length == 0)
            {
                // No (or multiple) object properties found: return 'null' so that no object properties are shown in the property grid
                return null;
            }

            if (filteredPropertyInfos.Length > 1)
            {
                // 4. We assume that the propertyInfos with AdditionalDataCheck are the most specific
                filteredPropertyInfos = filteredPropertyInfos.Where(pi => pi.AdditionalDataCheck != null).ToArray();
            }

            if (filteredPropertyInfos.Length == 1)
            {
                return CreateObjectProperties(filteredPropertyInfos.ElementAt(0), sourceData);
            }

            return null;
        }

        private static PropertyInfo[] FilterPropertyInfoByTypeInheritance(PropertyInfo[] propertyInfo, Func<PropertyInfo, Type> getTypeAction)
        {
            var propertyInfoCount = propertyInfo.Length;
            var propertyInfoWithUnInheritedType = propertyInfo.ToList();

            for (var i = 0; i < propertyInfoCount; i++)
            {
                var propertyToBeConsidered = propertyInfo[i];
                var firstType = getTypeAction(propertyToBeConsidered);

                for (var j = 0; j < propertyInfoCount; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var secondType = getTypeAction(propertyInfo[j]);

                    if (firstType != secondType && secondType.Implements(firstType))
                    {
                        propertyInfoWithUnInheritedType.Remove(propertyToBeConsidered);

                        break;
                    }
                }
            }

            return propertyInfoWithUnInheritedType.Any()
                       ? propertyInfoWithUnInheritedType.ToArray() // One or more specific property information objects found: return the filtered list
                       : propertyInfo; // No specific property information found: return the original list
        }

        private static object CreateObjectProperties(PropertyInfo propertyInfo, object sourceData)
        {
            try
            {
                // Try to create object properties for the source data
                var objectProperties = propertyInfo.CreateObjectProperties(sourceData);

                // Return a dynamic property bag containing the created object properties
                return new DynamicPropertyBag(objectProperties);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}