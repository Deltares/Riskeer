using System;
using System.Collections.Generic;
using System.Linq;
using DelftTools.Shell.Gui;
using DelftTools.Utils.PropertyBag.Dynamic;

namespace DeltaShell.Gui.Forms.PropertyGridView
{
    /// <summary>
    /// Helper class for resolving <see cref="PropertyInfo"/> objects.
    /// </summary>
    public static class PropertyResolver
    {
        /// <summary>
        /// Returns object properties based on the provided <paramref name="propertyInfos"/> and <paramref name="sourceData"/>.
        /// </summary>
        /// <param name="propertyInfos">The list of property info objects to obtain the object properties from.</param>
        /// <param name="sourceData">The source data to get the object properties for.</param>
        /// <returns>An object properties object, or null when no relevant propwrties object is found.</returns>
        public static object GetObjectProperties(List<PropertyInfo> propertyInfos, object sourceData)
        {
            if (sourceData == null)
            {
                return null;
            }

            // 1. Match property information based on ObjectType and on AdditionalDataCheck
            propertyInfos = propertyInfos.Where(pi => pi.ObjectType.IsInstanceOfType(sourceData) && (pi.AdditionalDataCheck == null || pi.AdditionalDataCheck(sourceData))).ToList();

            // 2. Match property information based on object type inheritance
            propertyInfos = FilterPropertyInfoByTypeInheritance(propertyInfos, pi => pi.ObjectType);

            // 3. Match property information based on property type inheritance
            propertyInfos = FilterPropertyInfoByTypeInheritance(propertyInfos, pi => pi.PropertyType);

            if (propertyInfos.Count == 0)
            {
                // No (or multiple) object properties found: return 'null' so that no object properties are shown in the property grid
                return null;
            }

            if (propertyInfos.Count > 1)
            {
                // 4. We assume that the propertyInfos with AdditionalDataCheck are the most specific
                propertyInfos = propertyInfos.Where(pi => pi.AdditionalDataCheck != null).ToList();
            }

            if (propertyInfos.Count == 1)
            {
                return CreateObjectProperties(propertyInfos.ElementAt(0), sourceData);
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
                return objectProperties is DynamicPropertyBag
                           ? (object)objectProperties
                           : new DynamicPropertyBag(objectProperties);
                return new DynamicPropertyBag(objectProperties);
            }
            catch (Exception)
            {
                return sourceData;
            }
        }
    }
}
