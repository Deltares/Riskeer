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
using Core.Common.Base.Data;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttributeTest
    {
        [Test]
        public void Constructor_WithoutNamePropertyName_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(null,
                                                                                            nameof(TestObject.Unit),
                                                                                            nameof(TestObject.Value));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("namePropertyName", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutUnitPropertyName_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                            null,
                                                                                            nameof(TestObject.Value));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("unitPropertyName", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutValuePropertyName_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                            nameof(TestObject.Unit),
                                                                                            null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("valuePropertyName", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_CreatesNewInstance()
        {
            // Call
            var attribute = new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                             nameof(TestObject.Unit),
                                                                                             nameof(TestObject.Value));

            // Assert
            Assert.IsInstanceOf<Attribute>(attribute);
        }

        [Test]
        public void GetName_ValidProperties_ReturnsExpectedValue()
        {
            // Setup
            const string expectedName = "expectedName";
            const string expectedUnit = "kN";

            var attribute = new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                             nameof(TestObject.Unit),
                                                                                             nameof(TestObject.Value));

            // Call
            string name = attribute.GetName(new TestObject
            {
                Name = expectedName,
                Unit = expectedUnit
            });

            // Assert
            Assert.AreEqual($"{expectedName} [{expectedUnit}]", name);
        }

        [Test]
        public void GetName_InvalidNameProperty_ThrowsArgumentException()
        {
            // Setup
            var attribute = new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute("IDoNotExist",
                                                                                             nameof(TestObject.Unit),
                                                                                             nameof(TestObject.Value));

            // Call
            void Call() => attribute.GetName(new TestObject());

            // Assert
            const string expectedMessage = "Name property 'IDoNotExist' was not found on type TestObject.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void GetName_InvalidUnitProperty_ThrowsArgumentException()
        {
            // Setup
            var attribute = new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                             "IDoNotExist",
                                                                                             nameof(TestObject.Value));

            // Call
            void Call() => attribute.GetName(new TestObject());

            // Assert
            const string expectedMessage = "Unit property 'IDoNotExist' was not found on type TestObject.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }
        
        [Test]
        public void GetName_UnitNull_ThrowsArgumentException()
        {
            // Setup
            var attribute = new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                             nameof(TestObject.Unit),
                                                                                             nameof(TestObject.Value));

            // Call
            void Call() =>
                attribute.GetName(new TestObject
                {
                    Unit = null
                });

            // Assert
            const string expectedMessage = "Unit property 'Unit' was not of type string.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GetValue_WithObjectWithRoundedDoubleProperty_ReturnsValueOfProperty()
        {
            // Setup
            var roundedDoubleValue = new RoundedDouble(5, 5.12345);
            var attribute = new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                             nameof(TestObject.Unit),
                                                                                             nameof(TestObject.Value));

            // Call
            string value = attribute.GetValue(new TestObject
            {
                Value = roundedDoubleValue
            });

            // Assert
            const string expectedResult = "5,12345";
            Assert.AreEqual(expectedResult, value);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(5.0000, "5")]
        [TestCase(-2.12000, "-2,12")]
        [TestCase(1.24560, "1,2456")]
        [TestCase(120, "120")]
        public void GetValue_WithObjectWithRoundedDoubleProperty_ReturnsValueOfPropertyWithoutTrailingZeroes(double doubleValue, string expectedResult)
        {
            // Setup
            var roundedDoubleValue = new RoundedDouble(5, doubleValue);
            var attribute = new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                             nameof(TestObject.Unit),
                                                                                             nameof(TestObject.Value));

            // Call
            string value = attribute.GetValue(new TestObject
            {
                Value = roundedDoubleValue
            });

            // Assert
            Assert.AreEqual(expectedResult, value);
        }

        [Test]
        public void GetValue_WithObjectWithNonStringProperty_ThrowsArgumentException()
        {
            // Setup
            int expectedValue = new Random(21).Next(3, 50);

            var attribute = new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                             nameof(TestObject.Unit),
                                                                                             nameof(TestObject.NonRoundedDoubleValue));

            // Call
            void Call() =>
                attribute.GetValue(new TestObject
                {
                    NonRoundedDoubleValue = expectedValue
                });

            // Assert
            const string expectedMessage = "Value property 'NonRoundedDoubleValue' was not of type RoundedDouble.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void GetValue_InvalidValueProperty_ThrowsArgumentException()
        {
            // Setup
            var attribute = new KeyValueAsRoundedDoubleWithoutTrailingZeroesElementAttribute(nameof(TestObject.Name),
                                                                                             nameof(TestObject.Unit),
                                                                                             "IDoNotExist");

            // Call
            void Call() => attribute.GetValue(new TestObject());

            // Assert
            const string expectedMessage = "Value property 'IDoNotExist' was not found on type TestObject.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        private class TestObject
        {
            public string Name { get; set; }
            public string Unit { get; set; }
            public RoundedDouble Value { get; set; }
            public int NonRoundedDoubleValue { get; set; }
        }
    }
}