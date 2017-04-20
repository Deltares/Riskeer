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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Core.Common.TestUtil;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Core.Common.Utils.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Reflection
{
    [TestFixture]
    public class TypeUtilsTest
    {
        [Test]
        public void GetDisplayName_EnumWithoutResourcesAttribute_EnumToString()
        {
            // Setup
            var value = TestEnum.NoDisplayName;

            // Call
            string result = TypeUtils.GetDisplayName(value);

            // Assert
            Assert.AreEqual(value.ToString(), result);
        }

        [Test]
        public void GetDisplayName_EnumWithResourcesAttribute_ResourceText()
        {
            // Setup
            var value = TestEnum.HasDisplayName;

            // Call
            string result = TypeUtils.GetDisplayName(value);

            // Assert
            Assert.AreEqual(Resources.EnumStringResource, result);
        }

        [Test]
        public void GetDisplayName_ValueNotPartOfEnumMembers_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var value = (TestEnum) 999;

            // Call
            TestDelegate call = () => TypeUtils.GetDisplayName(value);

            // Assert
            var message = "The value of argument 'enumValue' (999) is invalid for Enum type 'TestEnum'.";
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
        public void GetMemberName_ExpressionOfFuncIsNull_ThrowsArgumentNullException()
        {
            // Call
            Expression<Func<TestClass, object>> expression = null;
            TestDelegate call = () => TypeUtils.GetMemberName(expression);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("expression", paramName);
        }

        [Test]
        public void GetMemberName_ExpressionOfActionIsNull_ThrowsArgumentNullException()
        {
            // Call
            Expression<Action<TestClass>> expression = null;
            TestDelegate call = () => TypeUtils.GetMemberName(expression);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("expression", paramName);
        }

        [Test]
        public void GetMemberName_PropertyExpression_ReturnPropertyName()
        {
            // Call
            string memberName = TypeUtils.GetMemberName<TestClass>(t => t.PublicPropertyPrivateSetter);

            // Assert
            Assert.AreEqual("PublicPropertyPrivateSetter", memberName);
        }

        [Test]
        public void GetMemberName_FieldExpression_ReturnFieldName()
        {
            // Call
            var testClass = new TestClass();
            string memberName = TypeUtils.GetMemberName<TestClass>(t => testClass.PublicField);

            // Assert
            Assert.AreEqual("PublicField", memberName);
        }

        [Test]
        public void GetMemberName_MethodCallExpression_ReturnMethodName()
        {
            // Call
            var testClass = new TestClass();
            string memberName = TypeUtils.GetMemberName<TestClass>(t => testClass.PublicMethod());

            // Assert
            Assert.AreEqual("PublicMethod", memberName);
        }

        [Test]
        public void GetMemberName_MethodExpressionOfDifferentType_ThrowArgumentException()
        {
            // Call
            TestDelegate call = () => TypeUtils.GetMemberName<TestClass>(t => new object());

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("'t => new Object()' is geen geldige expressie voor deze methode.", exception.Message);
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
        public void HasTypeConverter_PropertyWithoutTypeConverterAttribute_ReturnFalse()
        {
            // Call
            bool hasTypeConverter = TypeUtils.HasTypeConverter<TestClass, Int32Converter>(c => c.PublicPropertyPrivateSetter);

            // Assert
            Assert.IsFalse(hasTypeConverter);
        }

        [Test]
        public void HasTypeConverter_PropertyWithDifferentTypeConverterAttribute_ReturnFalse()
        {
            // Call
            bool hasTypeConverter = TypeUtils.HasTypeConverter<TestClass, Int32Converter>(c => c.PropertyWithTypeConverter);

            // Assert
            Assert.IsFalse(hasTypeConverter);
        }

        [Test]
        public void HasTypeConverter_PropertyWithMatchingTypeConverterAttribute_ReturnTrue()
        {
            // Call
            bool hasTypeConverter = TypeUtils.HasTypeConverter<TestClass, DoubleConverter>(c => c.PropertyWithTypeConverter);

            // Assert
            Assert.IsTrue(hasTypeConverter);
        }

        [Test]
        public void HasTypeConverter_TypeConverterAttributeInherited_ReturnTrue()
        {
            // Call
            bool hasTypeConverter = TypeUtils.HasTypeConverter<DerivedTestClass, DoubleConverter>(c => c.PropertyWithTypeConverter);

            // Assert
            Assert.IsTrue(hasTypeConverter);
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

            public double PublicPropertyPrivateSetter { get; private set; }

            [TypeConverter(typeof(DoubleConverter))]
            public virtual double PropertyWithTypeConverter { get; private set; }

            public object PublicMethod()
            {
                return this;
            }

            /// <summary>
            /// Method used in reflection for tests above
            /// </summary>
            private int PrivateMethod(int i)
            {
                return i * 2;
            }
        }

        private class DerivedTestClass : TestClass
        {
            public DerivedTestClass(int privateInt) : base(privateInt) {}

            public override double PropertyWithTypeConverter
            {
                get
                {
                    return base.PropertyWithTypeConverter;
                }
            }
        }
    }
}