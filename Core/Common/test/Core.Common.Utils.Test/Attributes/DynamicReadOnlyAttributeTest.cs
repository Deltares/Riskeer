using System;

using Core.Common.Utils.Attributes;
using Core.Common.Utils.Test.Attributes.TestCaseClasses;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Attributes
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
        public void IsDynamicReadOnly_NoPropertyName_ReturnFalse(string propertyName)
        {
            // Call
            var isReadOnly = DynamicReadOnlyAttribute.IsDynamicReadOnly(new object(), propertyName);

            // Assert
            Assert.IsFalse(isReadOnly);
        }

        [Test]
        public void IsDynamicReadOnly_GivenPropertyNameDoesNotExistOnObject_ThrowMissingMemberException()
        {
            // Setup
            var o = new object();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsDynamicReadOnly(o, "NotExistingProperty");

            // Assert
            var exceptionMessage = Assert.Throws<MissingMemberException>(call).Message;
            Assert.AreEqual(string.Format("Kon eigenschap NotExistingProperty van type {0} niet vinden.", o.GetType()), exceptionMessage);
        }

        [Test]
        public void IsDynamicReadOnly_GivenPropertyDoesNotHaveDynamicReadOnlyAttribute_ReturnFalse()
        {
            // Setup
            var o = new ClassWithPropertyWithoutDynamicReadOnlyAttribute();

            // Call
            var isReadOnly = DynamicReadOnlyAttribute.IsDynamicReadOnly(o, "Property");

            // Assert
            Assert.IsFalse(isReadOnly);
        }

        [Test]
        public void IsDynamicReadOnly_ClassLacksDynamicReadOnlyValidationMethod_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButNoValidationMethod();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsDynamicReadOnly(o, "Property");

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicReadOnlyValidationMethod niet gevonden (of geen 'public' toegankelijkheid). Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsDynamicReadOnly_ClassHasMultipleDynamicReadOnlyValidationMethods_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyAndMultipleValidationMethod();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsDynamicReadOnly(o, "Property");

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Slechts één DynamicReadOnlyValidationMethod toegestaan per klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsDynamicReadOnly_ClassHasDynamicReadOnlyValidationMethodWithNonBoolReturnType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButValidationMethodReturnsIncorrectValueType();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsDynamicReadOnly(o, "Property");

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicReadOnlyValidationMethod moet 'bool' als 'return type' hebben. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsDynamicReadOnly_ClassHasDynamicReadOnlyValidationMethodWithIncorrectArgumentCount_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButValidationMethodNotOneArgument();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsDynamicReadOnly(o, "Property");

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicReadOnlyValidationMethod heeft een incorrect aantal argumenten. Zou er één moeten zijn. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsDynamicReadOnly_ClassHasDynamicReadOnlyValidationMethodWithIncorrectArgumentType_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicReadOnlyPropertyButValidationMethodArgumentNotString();

            // Call
            TestDelegate call = () => DynamicReadOnlyAttribute.IsDynamicReadOnly(o, "Property");

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Argument van DynamicReadOnlyValidationMethod moet van het type 'string' zijn. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsDynamicReadOnly_ClassWithDynamicReadOnlyProperty_ReturnResultFromValidationMethod(bool isReadOnly)
        {
            // Setup
            var o = new ClassWithDynamicReadOnlyProperty(isReadOnly);

            // Call
            var result = DynamicReadOnlyAttribute.IsDynamicReadOnly(o, "Property");

            // Assert
            Assert.AreEqual(isReadOnly, result);
        }
    }
}