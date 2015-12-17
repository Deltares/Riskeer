using System;

using Core.Common.Utils.Attributes;
using Core.Common.Utils.Test.Attributes.TestCaseClasses;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Attributes
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
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicVisibleValidationMethod niet gevonden (of geen 'public' toegankelijkheid). Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        public void CreateIsVisibleMethod_ClassHasMultipleDynamicVisibleValidationMethods_ThrowsMissingMethodException()
        {
            // Setup
            var o = new InvalidClassWithDynamicVisiblePropertyAndMultipleValidationMethod();

            // Call
            TestDelegate call = () => DynamicVisibleValidationMethodAttribute.CreateIsVisibleMethod(o);

            // Assert
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Slechts één DynamicVisibleValidationMethod toegestaan per klasse: {0}.",
                                                o.GetType());
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
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicVisibleValidationMethod moet 'bool' als 'return type' hebben. Klasse: {0}.",
                                                o.GetType());
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
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("DynamicVisibleValidationMethod heeft een incorrect aantal argumenten. Zou er één moeten zijn. Klasse: {0}.",
                                                o.GetType());
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
            var exceptionMessage = Assert.Throws<MissingMethodException>(call).Message;
            var expectedMessage = string.Format("Argument van DynamicVisibleValidationMethod moet van het type 'string' zijn. Klasse: {0}.",
                                                o.GetType());
            Assert.AreEqual(expectedMessage, exceptionMessage);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void CreateIsVisibleMethod_ClassWithDynamicVisibleProperty_ReturnResultFromValidationMethod(bool isReadOnly)
        {
            // Setup
            var o = new ClassWithDynamicVisibleProperty(isReadOnly);

            // Call
            var result = DynamicVisibleValidationMethodAttribute.CreateIsVisibleMethod(o);

            // Assert
            Assert.AreEqual(isReadOnly, result("Property"));
        }
    }
}