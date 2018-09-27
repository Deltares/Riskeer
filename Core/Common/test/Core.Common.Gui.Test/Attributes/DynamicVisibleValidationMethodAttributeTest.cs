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
    public class DynamicVisibleValidationMethodAttributeTest
    {
        [Test]
        public void CreateIsVisibleMethod_ClassLacksDynamicVisibleValidationMethod_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyButNoValidationMethod();

            // Call
            TestDelegate call = () => DynamicVisibleValidationMethodAttribute.CreateIsVisibleMethod(o);

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"DynamicVisibleValidationMethod niet gevonden (of geen 'public' toegankelijkheid). Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreateIsVisibleMethod_ClassHasMultipleDynamicVisibleValidationMethods_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyAndMultipleValidationMethods();

            // Call
            TestDelegate call = () => DynamicVisibleValidationMethodAttribute.CreateIsVisibleMethod(o);

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"Slechts één DynamicVisibleValidationMethod toegestaan per klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreateIsVisibleMethod_ClassHasDynamicVisibleValidationMethodWithNonBoolReturnType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyButValidationMethodReturnsIncorrectValueType();

            // Call
            TestDelegate call = () => DynamicVisibleValidationMethodAttribute.CreateIsVisibleMethod(o);

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"DynamicVisibleValidationMethod moet 'bool' als 'return type' hebben. Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreateIsVisibleMethod_ClassHasDynamicVisibleValidationMethodWithIncorrectArgumentCount_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyButValidationMethodNotOneArgument();

            // Call
            TestDelegate call = () => DynamicVisibleValidationMethodAttribute.CreateIsVisibleMethod(o);

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"DynamicVisibleValidationMethod heeft een incorrect aantal argumenten. Zou er één moeten zijn. Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreateIsVisibleMethod_ClassHasDynamicVisibleValidationMethodWithIncorrectArgumentType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyButValidationMethodArgumentNotString();

            // Call
            TestDelegate call = () => DynamicVisibleValidationMethodAttribute.CreateIsVisibleMethod(o);

            // Assert
            string exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            string expectedMessage = $"Argument van DynamicVisibleValidationMethod moet van het type 'string' zijn. Klasse: {o.GetType()}.";
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void CreateIsVisibleMethod_ClassWithDynamicVisibleProperty_ReturnResultFromValidationMethod(bool isVisible)
        {
            // Setup
            var o = new ClassWithDynamicVisibleProperty(isVisible);

            // Call
            DynamicVisibleValidationMethodAttribute.IsPropertyVisible result = DynamicVisibleValidationMethodAttribute.CreateIsVisibleMethod(o);

            // Assert
            Assert.AreEqual(isVisible, result("Property"));
        }
    }
}