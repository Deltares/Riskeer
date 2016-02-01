﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;

namespace Core.Common.Gui
{
    /// <summary>
    /// Extensions of <see cref="PropertyInfo"/>
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Creates object properties based on the combination of <paramref name="propertyInfo"/> and <paramref name="sourceData"/>
        /// </summary>
        /// <param name="propertyInfo">The property information used for creating the object properties</param>
        /// <param name="sourceData">The data that will be set to the created object properties instance</param>
        public static IObjectProperties CreateObjectProperties(this PropertyInfo propertyInfo, object sourceData)
        {
            var objectProperties = (IObjectProperties) Activator.CreateInstance(propertyInfo.PropertyType);

            objectProperties.Data = propertyInfo.GetObjectPropertiesData != null
                                        ? propertyInfo.GetObjectPropertiesData(sourceData)
                                        : sourceData;

            if (propertyInfo.AfterCreate != null)
            {
                propertyInfo.AfterCreate(objectProperties);
            }

            return objectProperties;
        }
    }
}