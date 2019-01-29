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
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class KeyValueElementAttributeTest
    {
        [Test]
        public void Constructor_WithoutValuePropertyName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new KeyValueElementAttribute("name", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("valuePropertyName", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutNamePropertyName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new KeyValueElementAttribute(null, "value");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("namePropertyName", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_CreatesNewInstance()
        {
            // Call
            var attribute = new KeyValueElementAttribute("name", "value");

            // Assert
            Assert.IsInstanceOf<Attribute>(attribute);
        }

        [Test]
        public void GetName_WithObjectWithProperty_ReturnsValueOfProperty()
        {
            // Setup
            const string expectedName = "expectedName";

            var attribute = new KeyValueElementAttribute(nameof(TestObject.Name), nameof(TestObject.Value));

            // Call
            string name = attribute.GetName(new TestObject
            {
                Name = expectedName
            });

            // Assert
            Assert.AreEqual(expectedName, name);
        }

        [Test]
        public void GetName_WithObjectWithNonStringProperty_ReturnsValueOfProperty()
        {
            // Setup
            int expectedName = new Random(21).Next(3, 50);

            var attribute = new KeyValueElementAttribute(nameof(TestObject.NonStringName), nameof(TestObject.NonStringValue));

            // Call
            string name = attribute.GetName(new TestObject
            {
                NonStringName = expectedName
            });

            // Assert
            Assert.AreEqual(Convert.ToString(expectedName), name);
        }

        [Test]
        public void GetName_WithObjectWithoutPropertyWithName_ThrowsArgumentException()
        {
            // Setup
            var attribute = new KeyValueElementAttribute("IDoNotExist", nameof(TestObject.Value));

            // Call
            TestDelegate test = () => attribute.GetName(new TestObject());

            // Assert
            const string expectedMessage = "Name property 'IDoNotExist' was not found on type TestObject.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void GetValue_WithObjectWithProperty_ReturnsValueOfProperty()
        {
            // Setup
            const string expectedValue = "expectedValue";

            var attribute = new KeyValueElementAttribute(nameof(TestObject.Name), nameof(TestObject.Value));

            // Call
            string value = attribute.GetValue(new TestObject
            {
                Value = expectedValue
            });

            // Assert
            Assert.AreEqual(expectedValue, value);
        }

        [Test]
        public void GetValue_WithObjectWithNonStringProperty_ReturnsStringConvertedValueOfProperty()
        {
            // Setup
            int expectedValue = new Random(21).Next(3, 50);

            var attribute = new KeyValueElementAttribute(nameof(TestObject.NonStringName), nameof(TestObject.NonStringValue));

            // Call
            string value = attribute.GetValue(new TestObject
            {
                NonStringValue = expectedValue
            });

            // Assert
            Assert.AreEqual(Convert.ToString(expectedValue), value);
        }

        [Test]
        public void GetValue_WithObjectWithoutPropertyWithName_ThrowsArgumentException()
        {
            // Setup
            var attribute = new KeyValueElementAttribute(nameof(TestObject.Name), "IDoNotExist");

            // Call
            TestDelegate test = () => attribute.GetValue(new TestObject());

            // Assert
            const string expectedMessage = "Value property 'IDoNotExist' was not found on type TestObject.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        private class TestObject
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public int NonStringName { get; set; }
            public int NonStringValue { get; set; }
        }
    }
}