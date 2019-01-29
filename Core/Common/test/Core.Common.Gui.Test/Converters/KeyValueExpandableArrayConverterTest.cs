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
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class KeyValueExpandableArrayConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new KeyValueExpandableArrayConverter();

            // Assert
            Assert.IsInstanceOf<ArrayConverter>(converter);
        }

        [Test]
        public void ConvertTo_FromArrayToString_ReturnCountText()
        {
            // Setup
            int arrayCount = new Random(21).Next(0, 10);

            var sourceArray = new int[arrayCount];
            var converter = new KeyValueExpandableArrayConverter();

            // Call
            object text = converter.ConvertTo(sourceArray, typeof(string));

            // Assert
            Assert.AreEqual($"Aantal ({arrayCount})", text);
        }

        [Test]
        public void ConvertTo_FromNullToString_ReturnEmptyText()
        {
            // Setup
            var converter = new KeyValueExpandableArrayConverter();

            // Call
            object text = converter.ConvertTo(null, typeof(string));

            // Assert
            Assert.AreEqual(string.Empty, text);
        }

        [Test]
        public void ConvertTo_FromArrayToInt_ThrowsNotSupportedException()
        {
            // Setup
            var sourceArray = new int[1];
            var converter = new KeyValueExpandableArrayConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(sourceArray, typeof(int));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertTo_FromArrayToNull_ThrowsArgumentNullException()
        {
            // Setup
            var sourceArray = new int[2];
            var converter = new KeyValueExpandableArrayConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(sourceArray, null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void GetProperties_WithoutContext_ThrowsArgumentException()
        {
            var converter = new KeyValueExpandableArrayConverter();

            // Call
            TestDelegate test = () => converter.GetProperties(new object());

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test,
                                                                                      "The KeyValueExpandableArrayConverter can only be used on properties that have the KeyValueElementAttribute defined.");
        }

        [Test]
        public void GetProperties_WithoutPropertyDescriptor_ThrowsArgumentException()
        {
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            var converter = new KeyValueExpandableArrayConverter();

            // Call
            TestDelegate test = () => converter.GetProperties(context, new object());

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test,
                                                                                      "The KeyValueExpandableArrayConverter can only be used on properties that have the KeyValueElementAttribute defined.");
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithoutKeyValueElementAttribute_ThrowsArgumentException()
        {
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            var descriptor = mocks.Stub<PropertyDescriptor>("name", new Attribute[0]);
            context.Stub(c => c.PropertyDescriptor).Return(descriptor);
            mocks.ReplayAll();

            var converter = new KeyValueExpandableArrayConverter();

            // Call
            TestDelegate test = () => converter.GetProperties(context, new object());

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                test,
                "The KeyValueExpandableArrayConverter can only be used on properties that have the KeyValueElementAttribute defined.");
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(12)]
        public void GetProperties_FromArray_ReturnPropertyDescriptorsForEachElementWithNameToOneBasedIndex(int elementCount)
        {
            // Setup
            var attribute = new KeyValueElementAttribute(nameof(TestObject.Name), nameof(TestObject.Value));
            var attributes = new AttributeCollection(attribute);

            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            var descriptor = mocks.Stub<PropertyDescriptor>("name", new Attribute[0]);
            descriptor.Stub(d => d.Attributes).Return(attributes);
            context.Stub(c => c.PropertyDescriptor).Return(descriptor);
            mocks.ReplayAll();

            const string name = "name";
            const string value = "value";
            TestObject[] array = Enumerable.Repeat(new TestObject
            {
                Name = name,
                Value = value
            }, elementCount).ToArray();

            var converter = new KeyValueExpandableArrayConverter();

            // Call
            PropertyDescriptorCollection propertyDescriptors = converter.GetProperties(context, array);

            // Assert
            Assert.IsNotNull(propertyDescriptors);
            Assert.AreEqual(elementCount, propertyDescriptors.Count);
            for (var i = 0; i < elementCount; i++)
            {
                Assert.AreEqual(typeof(TestObject), propertyDescriptors[i].ComponentType);
                Assert.AreEqual(name, propertyDescriptors[i].Name);
                Assert.AreEqual(name, propertyDescriptors[i].DisplayName);
                Assert.AreEqual(value.GetType(), propertyDescriptors[i].PropertyType);
                CollectionAssert.IsEmpty(propertyDescriptors[i].Attributes);

                var actualValue = propertyDescriptors[i].GetValue(array) as string;
                Assert.NotNull(actualValue);
                Assert.AreEqual(value, actualValue);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_FromArray_SettingValuesShouldThrowNotSupportedException()
        {
            // Setup
            var attribute = new KeyValueElementAttribute(nameof(TestObject.Name), nameof(TestObject.Value));
            var attributes = new AttributeCollection(attribute);

            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            var descriptor = mocks.Stub<PropertyDescriptor>("name", new Attribute[0]);
            descriptor.Stub(d => d.Attributes).Return(attributes);
            context.Stub(c => c.PropertyDescriptor).Return(descriptor);
            mocks.ReplayAll();

            const int elementCount = 12;
            TestObject[] array = Enumerable.Repeat(new TestObject
            {
                Name = "some name"
            }, elementCount).ToArray();

            var converter = new KeyValueExpandableArrayConverter();

            // Call
            PropertyDescriptorCollection propertyDescriptors = converter.GetProperties(context, array);

            // Assert
            for (var i = 0; i < elementCount; i++)
            {
                Assert.Throws<NotSupportedException>(() => propertyDescriptors[i].SetValue(array, i));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_IncorrectAttributeNamePropertyName_ThrowsArgumentException()
        {
            // Setup
            var attribute = new KeyValueElementAttribute("IDoNotExist", nameof(TestObject.Value));
            var attributes = new AttributeCollection(attribute);

            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            var descriptor = mocks.Stub<PropertyDescriptor>("name", new Attribute[0]);
            descriptor.Stub(d => d.Attributes).Return(attributes);
            context.Stub(c => c.PropertyDescriptor).Return(descriptor);
            mocks.ReplayAll();

            const int elementCount = 12;
            TestObject[] array = Enumerable.Repeat(new TestObject
            {
                Name = "some name"
            }, elementCount).ToArray();

            var converter = new KeyValueExpandableArrayConverter();

            // Call
            TestDelegate test = () => converter.GetProperties(context, array);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void GetProperties_IncorrectAttributeValuePropertyName_ThrowsArgumentException()
        {
            // Setup
            var attribute = new KeyValueElementAttribute(nameof(TestObject.Name), "IDoNotExist");
            var attributes = new AttributeCollection(attribute);

            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            var descriptor = mocks.Stub<PropertyDescriptor>("name", new Attribute[0]);
            descriptor.Stub(d => d.Attributes).Return(attributes);
            context.Stub(c => c.PropertyDescriptor).Return(descriptor);
            mocks.ReplayAll();

            const int elementCount = 12;
            TestObject[] array = Enumerable.Repeat(new TestObject
            {
                Name = "some name"
            }, elementCount).ToArray();

            var converter = new KeyValueExpandableArrayConverter();

            // Call
            TestDelegate test = () => converter.GetProperties(context, array);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        private class TestObject
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}