using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Properties;
using Core.Common.Utils.PropertyBag.Dynamic;

namespace Core.Common.Gui.Forms.PropertyGridView
{
    /// <summary>
    /// Helper class for resolving object properties.
    /// </summary>
    public class PropertyResolver : IPropertyResolver
    {
        private readonly List<PropertyInfo> propertyInfos;

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
            this.propertyInfos = propertyInfos.ToList();
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
            var filteredPropertyInfos = propertyInfos.Where(pi => pi.ObjectType.IsInstanceOfType(sourceData) && (pi.AdditionalDataCheck == null || pi.AdditionalDataCheck(sourceData))).ToList();

            // 2. Match property information based on object type inheritance
            filteredPropertyInfos = FilterPropertyInfoByTypeInheritance(filteredPropertyInfos, pi => pi.ObjectType);

            // 3. Match property information based on property type inheritance
            filteredPropertyInfos = FilterPropertyInfoByTypeInheritance(filteredPropertyInfos, pi => pi.PropertyType);

            if (filteredPropertyInfos.Count == 0)
            {
                // No (or multiple) object properties found: return 'null' so that no object properties are shown in the property grid
                return null;
            }

            if (filteredPropertyInfos.Count > 1)
            {
                // 4. We assume that the propertyInfos with AdditionalDataCheck are the most specific
                filteredPropertyInfos = filteredPropertyInfos.Where(pi => pi.AdditionalDataCheck != null).ToList();
            }

            if (filteredPropertyInfos.Count == 1)
            {
                return CreateObjectProperties(filteredPropertyInfos.ElementAt(0), sourceData);
            }

            return null;
        }

        private static List<PropertyInfo> FilterPropertyInfoByTypeInheritance(List<PropertyInfo> propertyInfo, Func<PropertyInfo, Type> getTypeAction)
        {
            var propertyInfoCount = propertyInfo.Count;
            var propertyInfoWithUnInheritedType = propertyInfo.ToList();

            for (var i = 0; i < propertyInfoCount; i++)
            {
                var firstType = getTypeAction(propertyInfo.ElementAt(i));

                for (var j = 0; j < propertyInfoCount; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var secondType = getTypeAction(propertyInfo.ElementAt(j));

                    if (firstType != secondType && firstType.IsAssignableFrom(secondType))
                    {
                        propertyInfoWithUnInheritedType.Remove(propertyInfo.ElementAt(i));

                        break;
                    }
                }
            }

            return propertyInfoWithUnInheritedType.Any()
                       ? propertyInfoWithUnInheritedType.ToList() // One or more specific property information objects found: return the filtered list
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