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

            var newDataType = typeof(object);
            var newPropertyObjectType = typeof(TestObjectProperties);
            Func<object, bool> newAdditionalDataDelegate = o => true;

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
        public void CreateInstance_ViewtypeWithDefaultConstructor_ReturnView()
        {
            // Setup
            var info = new PropertyInfo
            {
                DataType = typeof(int),
                PropertyObjectType = typeof(TestObjectProperties),
            };

            // Call
            object properties = info.CreateInstance(new Random(21).Next());

            // Assert
            Assert.IsInstanceOf<TestObjectProperties>(properties);
        }

        [Test]
        public void CreateInstance_ViewTypeHasDefaultConstructor_ReturnView()
        {
            // Setup
            var info = new PropertyInfo<int, TestObjectProperties>();

            // Call
            TestObjectProperties properties = info.CreateInstance(new Random(21).Next());

            // Assert
            Assert.IsNotNull(properties);
        }

        [Test]
        public void ImplicitOperator_OptionalDelegatesSet_PropertyInfoFullyConverted()
        {
            // Setup
            var info = new PropertyInfo<int, TestObjectProperties>();

            const int inputData = 42;
            var testProperties = new TestObjectProperties();

            // Precondition
            Assert.IsInstanceOf<PropertyInfo<int, TestObjectProperties>>(info);

            // Call
            PropertyInfo convertedInfo = info;

            // Assert
            Assert.IsInstanceOf<PropertyInfo>(convertedInfo);
            Assert.AreEqual(typeof(int), convertedInfo.DataType);
            Assert.AreEqual(typeof(TestObjectProperties), convertedInfo.PropertyObjectType);
        }

        [Test]
        public void ImplicitOperator_NoneOfTheOptionalDelegatesSet_PropertyInfoFullyConverted()
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