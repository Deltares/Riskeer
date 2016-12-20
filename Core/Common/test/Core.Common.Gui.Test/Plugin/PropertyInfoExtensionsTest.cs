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
        public void CreateObjectProperties_PropertyInfoCustomCreateInstance_CreateInstanceCalled()
        {
            // Setup
            int createInstanceCalled = 0;
            var testIntProperties = new TestIntProperties();
            PropertyInfo propertyInfo = new PropertyInfo<int, TestIntProperties>
            {
                CreateInstance = i =>
                {
                    createInstanceCalled++;
                    return testIntProperties;
                }
            };

            const int integerValue = 22;

            // Call
            var properties = propertyInfo.CreateObjectProperties(integerValue);

            // Assert
            Assert.AreSame(testIntProperties, properties);
            Assert.AreEqual(1, createInstanceCalled);
        }

        [Test]
        public void CreateObjectProperties_WithDataNotSetInCreateInstance_CreateObjectPropertiesObjectWithSourceData()
        {
            // Setup
            var sourceData = new Random(21).Next();
            PropertyInfo propertyInfo = new PropertyInfo<int, TestIntProperties>();

            // Call
            var properties = propertyInfo.CreateObjectProperties(sourceData);

            // Assert
            Assert.IsInstanceOf<TestIntProperties>(properties);
            Assert.AreEqual(sourceData, properties.Data);
        }

        [Test]
        public void CreateObjectProperties_WithDataSetInCreateInstance_CreateObjectPropertiesObjectWithCustom()
        {
            // Setup
            var random = new Random(21);
            var sourceData = random.Next();
            var customData = random.Next();
            PropertyInfo propertyInfo = new PropertyInfo<int, TestIntProperties>
            {
                CreateInstance = o => new TestIntProperties
                {
                    Data = customData
                }
            };

            // Call
            var properties = propertyInfo.CreateObjectProperties(sourceData);

            // Assert
            Assert.IsInstanceOf<TestIntProperties>(properties);
            Assert.AreEqual(customData, properties.Data);
        }

        private class TestIntProperties : IObjectProperties
        {
            public object Data { get; set; }
        }
    }
}