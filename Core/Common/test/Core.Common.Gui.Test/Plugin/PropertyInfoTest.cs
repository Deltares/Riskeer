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
            Assert.IsNull(info.AdditionalDataCheck);
            Assert.IsNull(info.GetObjectPropertiesData);
            Assert.IsNull(info.AfterCreate);
        }

        [Test]
        public void SimpleProperties_SetValues_GetNewlySetValues()
        {
            // Setup
            var info = new PropertyInfo();

            var newDataType = typeof(object);
            var newPropertyObjectType = typeof(TestObjectProperties);
            Func<object, bool> newAdditionalDataDelegate = o => true;
            Func<object, object> newGetObjectPropertiesDataDelegate = o => new object();
            Action<IObjectProperties, object> newAfterCreateDelegate = (properties, data) =>
            {
                // Do something with the view
            };

            // Call
            info.DataType = newDataType;
            info.PropertyObjectType = newPropertyObjectType;
            info.AdditionalDataCheck = newAdditionalDataDelegate;
            info.GetObjectPropertiesData = newGetObjectPropertiesDataDelegate;
            info.AfterCreate = newAfterCreateDelegate;

            // Assert
            Assert.AreEqual(newDataType, info.DataType);
            Assert.AreEqual(newPropertyObjectType, info.PropertyObjectType);
            Assert.AreEqual(newAdditionalDataDelegate, info.AdditionalDataCheck);
            Assert.AreEqual(newGetObjectPropertiesDataDelegate, info.GetObjectPropertiesData);
            Assert.AreEqual(newAfterCreateDelegate, info.AfterCreate);
        }

        [Test]
        public void DefaultGenericConstructor_ExpectedValues()
        {
            // Call
            var info = new PropertyInfo<int, TestObjectProperties>();

            // Assert
            Assert.AreEqual(typeof(int), info.DataType);
            Assert.AreEqual(typeof(TestObjectProperties), info.PropertyObjectType);
            Assert.IsNull(info.AdditionalDataCheck);
            Assert.IsNull(info.GetObjectPropertiesData);
            Assert.IsNull(info.AfterCreate);
        }

        [Test]
        public void SimpleProperties_SetValuesForGenericPropertyInfo_GetNewlySetValues()
        {
            // Setup
            var info = new PropertyInfo<int, TestObjectProperties>();

            Func<int, bool> newAdditionalDataDelegate = o => true;
            Func<int, object> newGetObjectPropertiesDataDelegate = o => new object();
            Action<TestObjectProperties, int> newAfterCreateDelegate = (property, data) =>
            {
                // Do something with the view
            };

            // Call
            info.AdditionalDataCheck = newAdditionalDataDelegate;
            info.GetObjectPropertiesData = newGetObjectPropertiesDataDelegate;
            info.AfterCreate = newAfterCreateDelegate;

            // Assert
            Assert.AreEqual(newAdditionalDataDelegate, info.AdditionalDataCheck);
            Assert.AreEqual(newGetObjectPropertiesDataDelegate, info.GetObjectPropertiesData);
            Assert.AreEqual(newAfterCreateDelegate, info.AfterCreate);
        }

        [Test]
        public void ImplicitOperator_OptionalDelegatesSet_PropertyInfoFullyConverted()
        {
            // Setup
            var info = new PropertyInfo<int, TestObjectProperties>();

            const int inputData = 42;
            var testProperties = new TestObjectProperties();

            bool additionalDataDelegateCalled = false;
            Func<int, bool> newAdditionalDataDelegate = o =>
            {
                Assert.AreEqual(inputData, o);
                additionalDataDelegateCalled = true;
                return true;
            };
            var alternativeObject = new object();
            bool getObjectPropertiesDataDelegateCalled = false;
            Func<int, object> newGetObjectPropertiesDataDelegate = o =>
            {
                Assert.AreEqual(inputData, o);
                getObjectPropertiesDataDelegateCalled = true;
                return alternativeObject;
            };
            bool afterCreateDelegateCalled = false;
            Action<TestObjectProperties, int> newAfterCreateDelegate = (properties, data) =>
            {
                Assert.AreSame(testProperties, properties);
                Assert.AreEqual(inputData, data);
                afterCreateDelegateCalled = true;
            };

            info.AdditionalDataCheck = newAdditionalDataDelegate;
            info.GetObjectPropertiesData = newGetObjectPropertiesDataDelegate;
            info.AfterCreate = newAfterCreateDelegate;

            // Precondition
            Assert.IsInstanceOf<PropertyInfo<int, TestObjectProperties>>(info);

            // Call
            PropertyInfo convertedInfo = info;

            // Assert
            Assert.IsInstanceOf<PropertyInfo>(convertedInfo);
            Assert.AreEqual(typeof(int), convertedInfo.DataType);
            Assert.AreEqual(typeof(TestObjectProperties), convertedInfo.PropertyObjectType);

            Assert.IsNotNull(convertedInfo.AdditionalDataCheck);
            Assert.IsTrue(convertedInfo.AdditionalDataCheck(inputData));
            Assert.IsTrue(additionalDataDelegateCalled);

            Assert.IsNotNull(convertedInfo.GetObjectPropertiesData);
            Assert.AreSame(alternativeObject, convertedInfo.GetObjectPropertiesData(inputData));
            Assert.IsTrue(getObjectPropertiesDataDelegateCalled);

            Assert.IsNotNull(convertedInfo.AfterCreate);
            convertedInfo.AfterCreate(testProperties, inputData);
            Assert.IsTrue(afterCreateDelegateCalled);
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

            Assert.IsNull(convertedInfo.AdditionalDataCheck);
            Assert.IsNull(convertedInfo.GetObjectPropertiesData);
            Assert.IsNull(convertedInfo.AfterCreate);
        }

        private class TestObjectProperties : ObjectProperties<object> {}
    }
}