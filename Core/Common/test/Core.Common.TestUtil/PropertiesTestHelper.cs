// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.ComponentModel;
using System.Windows.Forms;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Helper class for dealing with <see cref="IObjectProperties"/> implementations and
    /// other objects that are meant to be shown in the <see cref="PropertyGrid"/>.
    /// </summary>
    public static class PropertiesTestHelper
    {
        /// <summary>
        /// Gets all visible property descriptors for a given 'object properties' object.
        /// </summary>
        /// <param name="propertiesObject">The properties object.</param>
        /// <returns>All visible property descriptors.</returns>
        public static PropertyDescriptorCollection GetAllVisiblePropertyDescriptors(object propertiesObject)
        {
            var dynamicPropertyBag = new DynamicPropertyBag(propertiesObject);
            return dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });
        }

        /// <summary>
        /// Asserts the properties of a <see cref="PropertyDescriptor"/> on required subjects.
        /// </summary>
        /// <param name="property">The property to be checked.</param>
        /// <param name="expectedCategory">The expected category.</param>
        /// <param name="expectedDisplayName">The expected name of the property shown to the user.</param>
        /// <param name="expectedDescription">The expected description of the property shown to the user.</param>
        /// <param name="isReadOnly">Indicates whether or not the property is read-only.</param>
        public static void AssertRequiredPropertyDescriptorProperties(PropertyDescriptor property,
                                                                      string expectedCategory,
                                                                      string expectedDisplayName,
                                                                      string expectedDescription,
                                                                      bool isReadOnly = false)
        {
            Assert.AreEqual(isReadOnly, property.IsReadOnly);
            Assert.AreEqual(expectedCategory, property.Category);
            Assert.AreEqual(expectedDisplayName, property.DisplayName);
            Assert.AreEqual(expectedDescription, property.Description);
        }
    }
}