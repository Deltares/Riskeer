// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Reflection;

namespace Core.Common.Gui.Forms.PropertyGridView
{
    /// <summary>
    /// Class responsible for finding the <see cref="IObjectProperties"/> that has been
    /// registered for a given data-object.
    /// </summary>
    public class PropertyResolver : IPropertyResolver
    {
        private readonly IEnumerable<PropertyInfo> propertyInfos;

        /// <summary>
        /// Creates a new instance of <see cref="PropertyResolver"/> with the given <paramref name="propertyInfos"/>.
        /// </summary>
        /// <param name="propertyInfos">The list of property information objects to obtain the object properties from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyInfos"/> is <c>null</c>.</exception>
        public PropertyResolver(IEnumerable<PropertyInfo> propertyInfos)
        {
            if (propertyInfos == null)
            {
                throw new ArgumentNullException(nameof(propertyInfos), Resources.PropertyResolver_PropertyResolver_Cannot_create_PropertyResolver_without_list_of_PropertyInfo);
            }

            this.propertyInfos = propertyInfos.ToArray();
        }

        public object GetObjectProperties(object sourceData)
        {
            if (sourceData == null)
            {
                return null;
            }

            // 1. Match property information based on ObjectType and on AdditionalDataCheck:
            PropertyInfo[] filteredPropertyInfos = propertyInfos.Where(pi => pi.DataType.IsInstanceOfType(sourceData)).ToArray();

            // 2. Match property information based on object type inheritance, prioritizing most specialized object types:
            filteredPropertyInfos = FilterPropertyInfoByTypeInheritance(filteredPropertyInfos, pi => pi.DataType);

            // 3. Match property information based on property type inheritance, prioritizing most specialized object property types:
            filteredPropertyInfos = FilterPropertyInfoByTypeInheritance(filteredPropertyInfos, pi => pi.PropertyObjectType);

            if (filteredPropertyInfos.Length == 0)
            {
                // No object properties found: return 'null' so that no object properties are shown in the property grid
                return null;
            }

            if (filteredPropertyInfos.Length == 1)
            {
                return CreateObjectProperties(filteredPropertyInfos[0], sourceData);
            }

            return null;
        }

        private static PropertyInfo[] FilterPropertyInfoByTypeInheritance(PropertyInfo[] propertyInfo, Func<PropertyInfo, Type> getTypeAction)
        {
            int propertyInfoCount = propertyInfo.Length;
            List<PropertyInfo> propertyInfoWithUnInheritedType = propertyInfo.ToList();

            for (var i = 0; i < propertyInfoCount; i++)
            {
                PropertyInfo propertyToBeConsidered = propertyInfo[i];
                Type firstType = getTypeAction(propertyToBeConsidered);

                for (var j = 0; j < propertyInfoCount; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Type secondType = getTypeAction(propertyInfo[j]);

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
                IObjectProperties objectProperties = propertyInfo.CreateInstance(sourceData);

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