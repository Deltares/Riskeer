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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Core.Common.TestUtil;
using Core.Common.Util.Attributes;
using Core.Common.Util.Reflection;
using Core.Common.Util.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Util.Test.Reflection
{
    [TestFixture]
    public class TypeUtilsTest
    {
        [Test]
        public void GetDisplayName_EnumWithoutResourcesAttribute_EnumToString()
        {
            // Setup
            const TestEnum value = TestEnum.NoDisplayName;

            // Call
            string result = TypeUtils.GetDisplayName(value);

            // Assert
            Assert.AreEqual(value.ToString(), result);
        }

        [Test]
        public void GetDisplayName_EnumWithResourcesAttribute_ResourceText()
        {
            // Setup
            const TestEnum value = TestEnum.HasDisplayName;

            // Call
            string result = TypeUtils.GetDisplayName(value);

            // Assert
            Assert.AreEqual(Resources.EnumStringResource, result);
        }

        [Test]
        public void GetDisplayName_ValueNotPartOfEnumMembers_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const TestEnum value = (TestEnum) 999;

            // Call
            TestDelegate call = () => TypeUtils.GetDisplayName(value);

            // Assert
            const string message = "The value of argument 'enumValue' (999) is invalid for Enum type 'TestEnum'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, message);
            Assert.AreEqual("enumValue", exception.ParamName);
        }

        [Test]
        public void Implements_TypeNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => GetType().Implements(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("type", paramName);
        }

        [Test]
        public void Implements_TypeIsSameClassAsGenericType_ReturnTrue()
        {
            // Call
            bool isTypeUtilsTest = GetType().Implements<TypeUtilsTest>();

            // Assert
            Assert.IsTrue(isTypeUtilsTest);
        }

        [Test]
        public void Implements_SuperTypeComparedWithGenericTypeAsImplementedSubType_ReturnFalse()
        {
            // Call
            bool isTypeUtilsTest = typeof(object).Implements<TypeUtilsTest>();

            // Assert
            Assert.IsFalse(isTypeUtilsTest);
        }

        [Test]
        public void Implements_SubTypeComparedWithGenericTypeAsImplementedSuperType_ReturnTrue()
        {
            // Call
            bool isTypeUtilsTest = typeof(TypeUtilsTest).Implements<object>();

            // Assert
            Assert.IsTrue(isTypeUtilsTest);
        }

        [Test]
        public void Implements_TypeIsSameClassAsType_ReturnTrue()
        {
            // Setup
            Type type = GetType();

            // Call
            bool isTypeUtilsTest = type.Implements(typeof(TypeUtilsTest));

            // Assert
            Assert.IsTrue(isTypeUtilsTest);
        }

        [Test]
        public void Implements_SuperTypeComparedWitTypeAsImplementedSubType_ReturnFalse()
        {
            // Call
            bool isTypeUtilsTest = typeof(object).Implements(typeof(TypeUtilsTest));

            // Assert
            Assert.IsFalse(isTypeUtilsTest);
        }

        [Test]
        public void Implements_SubTypeComparedWithTypeAsImplementedSuperType_ReturnTrue()
        {
            // Call
            bool isTypeUtilsTest = typeof(TypeUtilsTest).Implements(typeof(object));

            // Assert
            Assert.IsTrue(isTypeUtilsTest);
        }

        [Test]
        public void GetField_InstanceNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => TypeUtils.GetField<int>(null, "A");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("instance", paramName);
        }

        [Test]
        public void GetField_FieldNameIsNull_ThrowArgumentNullException()
        {
            // Setup
            var testClass = new TestClass(22);

            // Call
            TestDelegate call = () => TypeUtils.GetField<int>(testClass, null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void GetField_PrivateField_ReturnFieldValue()
        {
            // Setup
            var testClass = new TestClass(22);

            // Call
            var privateIntValue = TypeUtils.GetField<int>(testClass, "privateInt");

            // Assert
            Assert.AreEqual(22, privateIntValue);
        }

        [Test]
        public void GetField_PrivateFieldOfDerivedClass_ReturnFieldValue()
        {
            // Setup
            var testClass = new DerivedTestClass(55);

            // Call
            var privateIntValue = TypeUtils.GetField<int>(testClass, "privateInt");

            // Assert
            Assert.AreEqual(55, privateIntValue);
        }

        [Test]
        public void GetField_PublicField_ReturnPublicFieldValue()
        {
            // Setup
            var testClass = new TestClass
            {
                PublicField = 1234
            };

            // Call
            var publicFieldValue = TypeUtils.GetField<int>(testClass, "PublicField");

            // Assert
            Assert.AreEqual(testClass.PublicField, publicFieldValue);
        }

        [Test]
        public void GetField_PublicFieldFromBaseClass_ReturnPublicFieldValue()
        {
            // Setup
            var derivedTestClass = new DerivedTestClass(1)
            {
                PublicField = 2
            };

            // Call
            var publicFieldValue = TypeUtils.GetField<int>(derivedTestClass, "PublicField");

            // Assert
            Assert.AreEqual(derivedTestClass.PublicField, publicFieldValue);
        }

        [Test]
        public void GetField_GetNonExistingPrivateField_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var testClass = new TestClass(0);

            // Call
            TestDelegate call = () => TypeUtils.GetField<int>(testClass, "nonExistingField");

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        public void GetProperty_InstanceNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => TypeUtils.GetProperty<int>(null, "A");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("instance", paramName);
        }

        [Test]
        public void GetProperty_PropertyNameIsNull_ThrowArgumentNullException()
        {
            // Setup
            var testClass = new TestClass(22);

            // Call
            TestDelegate call = () => TypeUtils.GetProperty<int>(testClass, null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void GetProperty_ProtectedProperty_ReturnPropertyValue()
        {
            // Setup
            var testClass = new TestClass();

            // Call
            var privateIntValue = TypeUtils.GetProperty<int>(testClass, "ProtectedIntProperty");

            // Assert
            Assert.AreEqual(102, privateIntValue);
        }

        [Test]
        public void GetProperty_ProtectedPropertyOfDerivedClass_ReturnPropertyValue()
        {
            // Setup
            var testClass = new DerivedTestClass();

            // Call
            var privateIntValue = TypeUtils.GetProperty<int>(testClass, "ProtectedIntProperty");

            // Assert
            Assert.AreEqual(102, privateIntValue);
        }

        [Test]
        public void GetProperty_PublicProperty_ReturnPublicPropertyValue()
        {
            // Setup
            var testClass = new TestClass
            {
                PublicProperty = 1234.0
            };

            // Call
            var publicPropertyValue = TypeUtils.GetProperty<double>(testClass, "PublicProperty");

            // Assert
            Assert.AreEqual(testClass.PublicProperty, publicPropertyValue);
        }

        [Test]
        public void GetProperty_PublicPropertyFromBaseClass_ReturnPublicPropertyValue()
        {
            // Setup
            var derivedTestClass = new DerivedTestClass(1)
            {
                PublicProperty = 2.0
            };

            // Call
            var publicPropertyValue = TypeUtils.GetProperty<double>(derivedTestClass, "PublicProperty");

            // Assert
            Assert.AreEqual(derivedTestClass.PublicProperty, publicPropertyValue);
        }

        [Test]
        public void GetProperty_GetNonExistingPrivateProperty_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var testClass = new TestClass(0);

            // Call
            TestDelegate call = () => TypeUtils.GetProperty<int>(testClass, "nonExistingProperty");

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        public void SetField_InstanceNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => TypeUtils.SetField(null, "A", "B");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("obj", paramName);
        }

        [Test]
        public void SetField_FieldNameIsNull_ThrowArgumentNullException()
        {
            // Setup
            var testClass = new TestClass(22);

            // Call
            TestDelegate call = () => TypeUtils.SetField(testClass, null, "B");

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void SetField_SettingPrivateField_FieldHasBeenUpdated()
        {
            // Setup
            var testClass = new TestClass(22);

            // Call
            TypeUtils.SetField(testClass, "privateInt", 23);

            // Assert
            Assert.AreEqual(23, TypeUtils.GetField<int>(testClass, "privateInt"));
        }

        [Test]
        public void SetField_SettingPrivateFieldWithIncorrectValueType_ThrowArgumentException()
        {
            // Setup
            var testClass = new TestClass(22);

            // Call
            TestDelegate call = () => TypeUtils.SetField(testClass, "privateInt", new object());

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void SetField_SetPrivateFieldOfBaseClass_FieldHasBeenUpdated()
        {
            // Setup
            var derivedTestClass = new DerivedTestClass(0);

            // Call
            TypeUtils.SetField(derivedTestClass, "privateInt", 23);

            // Assert
            Assert.AreEqual(23, TypeUtils.GetField<int>(derivedTestClass, "privateInt"));
        }

        [Test]
        public void SetField_SetNonExistingPrivateField_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var testClass = new TestClass(0);

            // Call
            TestDelegate call = () => TypeUtils.SetField(testClass, "nonExistingField", 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        public void SetField_SetPrivateFieldFromBaseClass_FieldHasBeenSet()
        {
            // Setup
            var derivedTestClass = new DerivedTestClass(1);

            // Call
            TypeUtils.SetField(derivedTestClass, "privateInt", 10);

            // Assert
            Assert.AreEqual(10, TypeUtils.GetField<int>(derivedTestClass, "privateInt"));
        }

        [Test]
        public void TypedCallPrivateMethod_InstanceNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => TypeUtils.CallPrivateMethod<object>(null, "A");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("instance", paramName);
        }

        [Test]
        public void CallPrivateMethod_InstanceNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => TypeUtils.CallPrivateMethod(null, "A");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("instance", paramName);
        }

        [Test]
        public void CallPrivateMethod_MethodWithReturnValue_ReturnMethodReturnValue()
        {
            // Setup
            var instance = new TestClass();

            // Call
            var returnValue = TypeUtils.CallPrivateMethod<int>(instance, "PrivateMethod", 1);

            // Assert
            Assert.AreEqual(2, returnValue);
        }

        [Test]
        public void CallPrivateMethodWithReturnValue_MethodDoesNotExist_ThrowArgumentOutOfRangeException()
        {
            // Setup
            var instance = new TestClass();

            // Call
            TestDelegate call = () => TypeUtils.CallPrivateMethod<int>(instance, "IDontExist", 1);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        public void CallPrivateMethodWithReturnValue_MethodDoesNotGetEnoughArguments_ThrowArgumentException()
        {
            // Setup
            var instance = new TestClass();

            // Call
            TestDelegate call = () => TypeUtils.CallPrivateMethod<int>(instance, "PrivateMethod");

            // Assert
            Assert.Throws<TargetParameterCountException>(call);
        }

        [Test]
        public void CallPrivateMethodWithReturnValue_MethodNameNull_ThrowArgumentNullException()
        {
            // Setup
            var instance = new TestClass();

            // Call
            TestDelegate call = () => TypeUtils.CallPrivateMethod<int>(instance, null, 1);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void CallPrivateMethod_ValidMethod_CallWithoutExceptions()
        {
            // Setup
            var instance = new TestClass();

            // Call
            TestDelegate call = () => TypeUtils.CallPrivateMethod(instance, "PrivateMethod", 1);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void CallPrivateMethod_MethodDoesNotExist_ThrowArgumentOutOfRangeException()
        {
            // Setup
            var instance = new TestClass();

            // Call
            TestDelegate call = () => TypeUtils.CallPrivateMethod(instance, "IDontExist", 1);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        public void CallPrivateMethod_MethodDoesNotGetEnoughArguments_ThrowArgumentException()
        {
            // Setup
            var instance = new TestClass();

            // Call
            TestDelegate call = () => TypeUtils.CallPrivateMethod(instance, "PrivateMethod");

            // Assert
            Assert.Throws<TargetParameterCountException>(call);
        }

        [Test]
        public void CallPrivateMethod_MethodNameNull_ThrowArgumentNullException()
        {
            // Setup
            var instance = new TestClass();

            // Call
            TestDelegate call = () => TypeUtils.CallPrivateMethod(instance, null, 1);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void SetPrivatePropertyValue_InstanceNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => TypeUtils.SetPrivatePropertyValue(null, "A", "B");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("instance", paramName);
        }

        [Test]
        public void SetPrivatePropertyValue_PropertyNameIsNull_ThrowArgumentNullException()
        {
            // Setup
            var instance = new TestClass();

            // Call
            TestDelegate call = () => TypeUtils.SetPrivatePropertyValue(instance, null, "B");

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void SetPrivatePropertyValue_NotExistingProperty_ThrowArgumentOutOfRangeException()
        {
            // Setup
            var testClass = new TestClass(1);

            // Precondition
            Assert.AreEqual(0.0, testClass.PublicPropertyPrivateSetter);

            // Call
            TestDelegate call = () => TypeUtils.SetPrivatePropertyValue(testClass, "IDonNotExist", 1.2);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        public void SetPrivatePropertyValue_PublicPropertyPrivateSetter_PropertyIsSet()
        {
            // Setup
            var testClass = new TestClass(1);

            // Precondition
            Assert.AreEqual(0.0, testClass.PublicPropertyPrivateSetter);

            // Call
            TypeUtils.SetPrivatePropertyValue(testClass, "PublicPropertyPrivateSetter", 1.2);

            // Assert
            Assert.AreEqual(1.2, testClass.PublicPropertyPrivateSetter);
        }

        [Test]
        public void GetPropertyAttributes_PropertyNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => TypeUtils.GetPropertyAttributes<AttributeClass, TestingAttribute>(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertyName", paramName);
        }

        [Test]
        public void GetPropertyAttributes_WithPropertyName_ReturnsTestingAttributes()
        {
            // Call
            IEnumerable<TestingAttribute> attributes = TypeUtils.GetPropertyAttributes<AttributeClass, TestingAttribute>(nameof(AttributeClass.Property));

            // Assert
            Assert.AreEqual(2, attributes.Count());
            CollectionAssert.AllItemsAreInstancesOfType(attributes, typeof(TestingAttribute));
            Assert.AreEqual("attribute 1", attributes.First().Name);
            Assert.AreEqual("attribute 2", attributes.ElementAt(1).Name);
        }

        private enum TestEnum
        {
            NoDisplayName,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.EnumStringResource))]
            HasDisplayName
        }

        private class TestClass
        {
            /// <summary>
            /// Property used in reflection for tests above
            /// </summary>
            public int PublicField;

            /// <summary>
            /// Property used in reflection for tests above
            /// </summary>
            private int privateInt;

            public TestClass() {}

            public TestClass(int privateInt)
            {
                this.privateInt = privateInt;
            }

            public double PublicProperty { get; set; }

            public double PublicPropertyPrivateSetter { get; private set; }

            protected int ProtectedIntProperty
            {
                get
                {
                    return 102;
                }
            }

            private int PrivateMethod(int i)
            {
                return i * 2;
            }
        }

        private class DerivedTestClass : TestClass
        {
            public DerivedTestClass() {}

            public DerivedTestClass(int privateInt) : base(privateInt) {}
        }

        private class AttributeClass
        {
            [Testing(Name = "attribute 1")]
            [Testing(Name = "attribute 2")]
            public int Property { get; }
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        private class TestingAttribute : Attribute
        {
            public string Name { get; set; }
        }
    }
}