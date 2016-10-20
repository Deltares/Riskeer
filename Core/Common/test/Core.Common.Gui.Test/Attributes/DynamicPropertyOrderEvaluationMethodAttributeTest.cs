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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Test.Attributes.TestCaseClasses;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Attributes
{
    [TestFixture]
    public class DynamicPropertyOrderEvaluationMethodAttributeTest
    {
        [Test]
        public void CreatePropertyOrderMethod_ClassLacksDynamicPropertyOrderEvaluationMethod_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicPropertyOrderPropertyButNoEvaluationMethod();

            // Call
            TestDelegate call = () => DynamicPropertyOrderEvaluationMethodAttribute.CreatePropertyOrderMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicPropertyOrderEvaluationMethod niet gevonden (of geen 'public' toegankelijkheid). Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreatePropertyOrderMethod_ClassHasMultipleDynamicPropertyOrderEvaluationMethods_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicPropertyOrderPropertyAndMultipleEvaluationMethods();

            // Call
            TestDelegate call = () => DynamicPropertyOrderEvaluationMethodAttribute.CreatePropertyOrderMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Slechts één DynamicPropertyOrderEvaluationMethod toegestaan per klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreatePropertyOrderMethod_ClassHasDynamicPropertyOrderEvaluationMethodWithNonIntReturnType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicPropertyOrderPropertyButEvaluationMethodReturnsIncorrectValueType();

            // Call
            TestDelegate call = () => DynamicPropertyOrderEvaluationMethodAttribute.CreatePropertyOrderMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicPropertyOrderEvaluationMethod moet 'int' als 'return type' hebben. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreatePropertyOrderMethod_ClassHasDynamicPropertyOrderEvaluationMethodWithIncorrectArgumentCount_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicPropertyOrderPropertyButEvaluationMethodNotOneArgument();

            // Call
            TestDelegate call = () => DynamicPropertyOrderEvaluationMethodAttribute.CreatePropertyOrderMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicPropertyOrderEvaluationMethod heeft een incorrect aantal argumenten. Zou er één moeten zijn. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreatePropertyOrderMethod_ClassHasDynamicPropertyOrderEvaluationMethodWithIncorrectArgumentType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicPropertyOrderPropertyButEvaluationMethodArgumentNotString();

            // Call
            TestDelegate call = () => DynamicPropertyOrderEvaluationMethodAttribute.CreatePropertyOrderMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Argument van DynamicPropertyOrderEvaluationMethod moet van het type 'string' zijn. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void CreatePropertyOrderMethod_ClassWithDynamicPropertyOrderProperty_ReturnResultFromEvaluationMethod(int propertyOrder)
        {
            // Setup
            var o = new ClassWithDynamicPropertyOrderProperty(propertyOrder);

            // Call
            var result = DynamicPropertyOrderEvaluationMethodAttribute.CreatePropertyOrderMethod(o);

            // Assert
            Assert.AreEqual(propertyOrder, result("Property"));
        }
    }
}