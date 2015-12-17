using System;

using Core.Common.Utils.Attributes;
using Core.Common.Utils.Test.Attributes.TestCaseClasses;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Attributes
{
    [TestFixture]
    public class DynamicReadOnlyValidationMethodAttributeTest
    {
        [Test]
        public void CreateIsReadOnlyMethod_ClassLacksDynamicReadOnlyValidationMethod_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButNoValidationMethod();

            // Call
            TestDelegate call = () => DynamicReadOnlyValidationMethodAttribute.CreateIsReadOnlyMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicReadOnlyValidationMethod niet gevonden (of geen 'public' toegankelijkheid). Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreateIsReadOnlyMethod_ClassHasMultipleDynamicReadOnlyValidationMethods_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyAndMultipleValidationMethod();

            // Call
            TestDelegate call = () => DynamicReadOnlyValidationMethodAttribute.CreateIsReadOnlyMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Slechts één DynamicReadOnlyValidationMethod toegestaan per klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreateIsReadOnlyMethod_ClassHasDynamicReadOnlyValidationMethodWithNonBoolReturnType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButValidationMethodReturnsIncorrectValueType();

            // Call
            TestDelegate call = () => DynamicReadOnlyValidationMethodAttribute.CreateIsReadOnlyMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicReadOnlyValidationMethod moet 'bool' als 'return type' hebben. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreateIsReadOnlyMethod_ClassHasDynamicReadOnlyValidationMethodWithIncorrectArgumentCount_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButValidationMethodNotOneArgument();

            // Call
            TestDelegate call = () => DynamicReadOnlyValidationMethodAttribute.CreateIsReadOnlyMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicReadOnlyValidationMethod heeft een incorrect aantal argumenten. Zou er één moeten zijn. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreateIsReadOnlyMethod_ClassHasDynamicReadOnlyValidationMethodWithIncorrectArgumentType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButValidationMethodArgumentNotString();

            // Call
            TestDelegate call = () => DynamicReadOnlyValidationMethodAttribute.CreateIsReadOnlyMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Argument van DynamicReadOnlyValidationMethod moet van het type 'string' zijn. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void CreateIsReadOnlyMethod_ClassWithDynamicReadOnlyProperty_ReturnResultFromValidationMethod(bool isReadOnly)
        {
            // Setup
            var o = new ClassWithDynamicReadOnlyProperty(isReadOnly);

            // Call
            var result = DynamicReadOnlyValidationMethodAttribute.CreateIsReadOnlyMethod(o);

            // Assert
            Assert.AreEqual(isReadOnly, result("Property"));
        }
    }
}