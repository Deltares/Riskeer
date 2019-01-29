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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Test.Attributes.TestCaseClasses;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Attributes
{
    [TestFixture]
    public class DynamicVisibleAttributeTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var attribute = new DynamicVisibleAttribute();

            // Assert
            Assert.IsInstanceOf<Attribute>(attribute);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void IsVisible_NoPropertyName_ReturnTrue(string propertyName)
        {
            // Call
            bool isVisible = DynamicVisibleAttribute.IsVisible(new object(), propertyName);

            // Assert
            Assert.IsTrue(isVisible);
        }

        [Test]
        public void IsVisible_GivenPropertyNameDoesNotExistOnObject_ThrowMissingMemberException()
        {
            // Setup
            var o = new object();

            // Call
            TestDelegate call = () => DynamicVisibleAttribute.IsVisible(o, "NotExistingProperty");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMemberException>(call).Message;
            Assert.AreEqual($"Kon eigenschap NotExistingProperty van type {o.GetType()} niet vinden.", exceptionMessage);
        }

        [Test]
        public void IsVisible_GivenPropertyDoesNotHaveDynamicVisibleAttribute_ReturnTrue()
        {
            // Setup
            var o = new ClassWithPropertyWithoutDynamicVisibleAttribute();

            // Call
            bool isVisible = DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            Assert.IsTrue(isVisible);
        }

        [Test]
        public void IsVisible_ClassLacksDynamicVisibleValidationMethod_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyButNoValidationMethod();

            // Call
            TestDelegate call = () => DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"DynamicVisibleValidationMethod niet gevonden (of geen 'public' toegankelijkheid). Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsVisible_ClassHasMultipleDynamicVisibleValidationMethods_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyAndMultipleValidationMethods();

            // Call
            TestDelegate call = () => DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"Slechts één DynamicVisibleValidationMethod toegestaan per klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsVisible_ClassHasDynamicVisibleValidationMethodWithNonBoolReturnType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyButValidationMethodReturnsIncorrectValueType();

            // Call
            TestDelegate call = () => DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"DynamicVisibleValidationMethod moet 'bool' als 'return type' hebben. Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsVisible_ClassHasDynamicVisibleValidationMethodWithIncorrectArgumentCount_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyButValidationMethodNotOneArgument();

            // Call
            TestDelegate call = () => DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"DynamicVisibleValidationMethod heeft een incorrect aantal argumenten. Zou er één moeten zijn. Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsVisible_ClassHasDynamicVisibleValidationMethodWithIncorrectArgumentType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyButValidationMethodArgumentNotString();

            // Call
            TestDelegate call = () => DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"Argument van DynamicVisibleValidationMethod moet van het type 'string' zijn. Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsVisible_ClassWithDynamicVisibleProperty_ReturnResultFromValidationMethod(bool isVisible)
        {
            // Setup
            var o = new ClassWithDynamicVisibleProperty(isVisible);

            // Call
            bool result = DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            Assert.AreEqual(isVisible, result);
        }
    }
}