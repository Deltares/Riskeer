// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Test.Attributes.TestCaseClasses;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Attributes
{
    [TestFixture]
    public class DynamicReadOnlyAttributeTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var attribute = new DynamicReadOnlyAttribute();

            // Assert
            Assert.IsInstanceOf<Attribute>(attribute);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void IsReadOnly_NoPropertyName_ReturnFalse(string propertyName)
        {
            // Call
            bool isReadOnly = DynamicReadOnlyAttribute.IsReadOnly(new object(), propertyName);

            // Assert
            Assert.IsFalse(isReadOnly);
        }

        [Test]
        public void IsReadOnly_GivenPropertyNameDoesNotExistOnObject_ThrowMissingMemberException()
        {
            // Setup
            var o = new object();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsReadOnly(o, "NotExistingProperty");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMemberException>(call).Message;
            Assert.AreEqual($"Kon eigenschap NotExistingProperty van type {o.GetType()} niet vinden.", exceptionMessage);
        }

        [Test]
        public void IsReadOnly_GivenPropertyDoesNotHaveDynamicReadOnlyAttribute_ReturnFalse()
        {
            // Setup
            var o = new ClassWithPropertyWithoutDynamicReadOnlyAttribute();

            // Call
            bool isReadOnly = DynamicReadOnlyAttribute.IsReadOnly(o, "Property");

            // Assert
            Assert.IsFalse(isReadOnly);
        }

        [Test]
        public void IsReadOnly_ClassLacksDynamicReadOnlyValidationMethod_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButNoValidationMethod();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsReadOnly(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"DynamicReadOnlyValidationMethod niet gevonden (of geen 'public' toegankelijkheid). Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsReadOnly_ClassHasMultipleDynamicReadOnlyValidationMethods_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyAndMultipleValidationMethods();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsReadOnly(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"Slechts één DynamicReadOnlyValidationMethod toegestaan per klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsReadOnly_ClassHasDynamicReadOnlyValidationMethodWithNonBoolReturnType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButValidationMethodReturnsIncorrectValueType();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsReadOnly(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"DynamicReadOnlyValidationMethod moet 'bool' als 'return type' hebben. Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsReadOnly_ClassHasDynamicReadOnlyValidationMethodWithIncorrectArgumentCount_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButValidationMethodNotOneArgument();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsReadOnly(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"DynamicReadOnlyValidationMethod heeft een incorrect aantal argumenten. Zou er één moeten zijn. Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsReadOnly_ClassHasDynamicReadOnlyValidationMethodWithIncorrectArgumentType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButValidationMethodArgumentNotString();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsReadOnly(o, "Property");

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"Argument van DynamicReadOnlyValidationMethod moet van het type 'string' zijn. Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsReadOnly_ClassWithDynamicReadOnlyProperty_ReturnResultFromValidationMethod(bool isReadOnly)
        {
            // Setup
            var o = new ClassWithDynamicReadOnlyProperty(isReadOnly);

            // Call
            bool result = DynamicReadOnlyAttribute.IsReadOnly(o, "Property");

            // Assert
            Assert.AreEqual(isReadOnly, result);
        }
    }
}