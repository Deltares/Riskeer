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
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Plugin
{
    [TestFixture]
    public class PropertyInfoTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var info = new PropertyInfo();

            // Assert
            Assert.IsNull(info.DataType);
            Assert.IsNull(info.PropertyObjectType);
        }

        [Test]
        public void SimpleProperties_SetValues_GetNewlySetValues()
        {
            // Setup
            var info = new PropertyInfo();

            Type newDataType = typeof(object);
            Type newPropertyObjectType = typeof(TestObjectProperties);

            // Call
            info.DataType = newDataType;
            info.PropertyObjectType = newPropertyObjectType;

            // Assert
            Assert.AreEqual(newDataType, info.DataType);
            Assert.AreEqual(newPropertyObjectType, info.PropertyObjectType);
        }

        [Test]
        public void DefaultGenericConstructor_ExpectedValues()
        {
            // Call
            var info = new PropertyInfo<int, TestObjectProperties>();

            // Assert
            Assert.AreEqual(typeof(int), info.DataType);
            Assert.AreEqual(typeof(TestObjectProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_DataTypeWithDefaultConstructor_ReturnPropertiesWithDataSet()
        {
            // Setup
            var info = new PropertyInfo
            {
                DataType = typeof(int),
                PropertyObjectType = typeof(TestObjectProperties)
            };
            int data = new Random(21).Next();

            // Call
            object properties = info.CreateInstance(data);

            // Assert
            Assert.IsInstanceOf<TestObjectProperties>(properties);
            var testObjectProperties = (TestObjectProperties) properties;
            Assert.AreEqual(data, testObjectProperties.Data);
        }

        [Test]
        public void CreateInstanceGeneric_DataTypeHasDefaultConstructor_ReturnPropertiesWithDataSet()
        {
            // Setup
            var info = new PropertyInfo<int, TestObjectProperties>();
            int data = new Random(21).Next();

            // Call
            TestObjectProperties properties = info.CreateInstance(data);

            // Assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(data, properties.Data);
        }

        [Test]
        public void ImplicitOperator_Always_PropertyInfoFullyConverted()
        {
            // Setup
            var info = new PropertyInfo<int, TestObjectProperties>();

            // Precondition
            Assert.IsInstanceOf<PropertyInfo<int, TestObjectProperties>>(info);

            // Call
            PropertyInfo convertedInfo = info;

            // Assert
            Assert.IsInstanceOf<PropertyInfo>(convertedInfo);
            Assert.AreEqual(typeof(int), convertedInfo.DataType);
            Assert.AreEqual(typeof(TestObjectProperties), convertedInfo.PropertyObjectType);
        }

        private class TestObjectProperties : ObjectProperties<object> {}
    }
}