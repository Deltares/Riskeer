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
using System.ComponentModel;
using System.Reflection;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;

namespace Core.Common.Gui.Test.PropertyBag
{
    [TestFixture]
    public class ObjectPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new ObjectProperties<string>();

            // Assert
            Assert.IsInstanceOf<IObjectProperties>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetValue_GetNewlySetValue()
        {
            // Setup
            var properties = new ObjectProperties<string>();
            const string text = "text";

            // Call
            properties.Data = text;

            // Assert
            Assert.AreEqual(text, properties.Data);
        }

        [Test]
        public void Data_IsNotBrowsable()
        {
            // Setup
            var properties = new ObjectProperties<string>();
            const string dataPropertyName = nameof(ObjectProperties<string>.Data);
            PropertyInfo propertyInfo = properties.GetType().GetProperty(dataPropertyName);

            // Call
            var browsableAttribute = (BrowsableAttribute) Attribute.GetCustomAttribute(propertyInfo,
                                                                                       typeof(BrowsableAttribute),
                                                                                       true);

            // Assert
            Assert.AreEqual(BrowsableAttribute.No, browsableAttribute);
        }
    }
}