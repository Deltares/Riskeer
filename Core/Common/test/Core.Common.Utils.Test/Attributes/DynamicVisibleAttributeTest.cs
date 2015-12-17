using System;

using Core.Common.Utils.Attributes;
using Core.Common.Utils.Test.Attributes.TestCaseClasses;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Attributes
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
            var isReadOnly = DynamicVisibleAttribute.IsVisible(new object(), propertyName);

            // Assert
            Assert.IsTrue(isReadOnly);
        }

        [Test]
        public void IsVisible_GivenPropertyNameDoesNotExistOnObject_ThrowMissingMemberException()
        {
            // Setup
            var o = new object();

            // Call
            TestDelegate call = () => DynamicVisibleAttribute.IsVisible(o, "NotExistingProperty");

            // Assert
            var exceptionMessage = Assert.Throws<MissingMemberException>(call).Message;
            Assert.AreEqual(string.Format("Kon eigenschap NotExistingProperty van type {0} niet vinden.", o.GetType()), exceptionMessage);
        }

        [Test]
        public void IsVisible_GivenPropertyDoesNotHaveDynamicVisibleAttribute_ReturnTrue()
        {
            // Setup
            var o = new ClassWithPropertyWithoutDynamicVisibleAttribute();

            // Call
            var isReadOnly = DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            Assert.IsTrue(isReadOnly);
        }

        [Test]
        public void IsVisible_ClassLacksDynamicVisibleValidationMethod_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyButNoValidationMethod();

            // Call
            TestDelegate call = () => DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicVisibleValidationMethod niet gevonden (of geen 'public' toegankelijkheid). Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void IsVisible_ClassHasMultipleDynamicVisibleValidationMethods_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyAndMultipleValidationMethod();

            // Call
            TestDelegate call = () => DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Slechts één DynamicVisibleValidationMethod toegestaan per klasse: {0}.",
                                                o.GetType());
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
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicVisibleValidationMethod moet 'bool' als 'return type' hebben. Klasse: {0}.",
                                                o.GetType());
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
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicVisibleValidationMethod heeft een incorrect aantal argumenten. Zou er één moeten zijn. Klasse: {0}.",
                                                o.GetType());
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
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Argument van DynamicVisibleValidationMethod moet van het type 'string' zijn. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsVisible_ClassWithDynamicVisibleProperty_ReturnResultFromValidationMethod(bool isReadOnly)
        {
            // Setup
            var o = new ClassWithDynamicVisibleProperty(isReadOnly);

            // Call
            var result = DynamicVisibleAttribute.IsVisible(o, "Property");

            // Assert
            Assert.AreEqual(isReadOnly, result);
        }
    }
}