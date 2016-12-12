// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Plugin
{
    [TestFixture]
    public class PropertyInfoExtensionsTest
    {
        [Test]
        public void CreateObjectProperties_InfoNull_ThrowArgumentNullException()
        {
            // Setup
            PropertyInfo info = null;

            // Call
            TestDelegate call = () => info.CreateObjectProperties(345);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertyInfo", paramName);
        }

        [Test]
        public void CreateObjectProperties_SimplePropertyInfo_CreateObjectPropertiesObjectForData()
        {
            // Setup
            PropertyInfo propertyInfo = new PropertyInfo<int, TestIntProperties>();

            const int integerValue = 22;

            // Call
            var properties = propertyInfo.CreateObjectProperties(integerValue);

            // Assert
            Assert.IsInstanceOf<TestIntProperties>(properties);
            Assert.AreEqual(integerValue, properties.Data);
        }

        [Test]
        public void CreateObjectProperties_PropertyInfoWithGetObjectPropertiesData_CreateObjectPropertiesObjectForTransformedData()
        {
            // Setup
            const int alternativeIntegerValue = 13;
            PropertyInfo propertyInfo = new PropertyInfo<int, TestIntProperties>
            {
                GetObjectPropertiesData = i => alternativeIntegerValue
            };

            const int integerValue = 22;

            // Call
            var properties = propertyInfo.CreateObjectProperties(integerValue);

            // Assert
            Assert.IsInstanceOf<TestIntProperties>(properties);
            Assert.AreEqual(alternativeIntegerValue, properties.Data);
        }

        [Test]
        public void CreateObjectProperties_PropertyInfoWithGetObjectPropertiesDataAndAfterCreate_CreateObjectPropertiesObjectForTransformedData()
        {
            // Setup
            const int alternativeIntegerValue = 13;
            PropertyInfo propertyInfo = new PropertyInfo<int, TestIntProperties>
            {
                GetObjectPropertiesData = i => alternativeIntegerValue,
                AfterCreate = (intProperties, data) =>
                {
                    Assert.AreEqual(alternativeIntegerValue, intProperties.Data);
                }
            };

            const int integerValue = 22;

            // Call
            var properties = propertyInfo.CreateObjectProperties(integerValue);

            // Assert
            Assert.IsInstanceOf<TestIntProperties>(properties);
            Assert.AreEqual(alternativeIntegerValue, properties.Data);
        }

        private class TestIntProperties : IObjectProperties
        {
            public object Data { get; set; }
        }
    }
}