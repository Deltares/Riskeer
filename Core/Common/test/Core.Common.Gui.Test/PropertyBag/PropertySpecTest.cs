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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Gui.Test.PropertyBag
{
    [TestFixture]
    public class PropertySpecTest
    {
        [Test]
        public void Constructor_InfoNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PropertySpec(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertyInfo", paramName);
        }

        [Test]
        public void ParameteredConstructor_FromPropertyWithoutAttributesWithPublicGetSet_ExpectedValues()
        {
            // Setup
            const string propertyName = "IntegerProperty";
            PropertyInfo propertyInfo = typeof(ClassWithProperties).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            CollectionAssert.IsEmpty(propertySpec.Attributes);
        }

        [Test]
        public void ParameteredConstructor_FromPropertyWithoutAttributesWithOnlyPublicGet_ExpectedValues()
        {
            // Setup
            const string propertyName = "DoublePropertyWithOnlyPublicGet";
            PropertyInfo propertyInfo = typeof(ClassWithProperties).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            Assert.AreEqual(1, propertySpec.Attributes.Count);
            var readOnlyAttribute = (ReadOnlyAttribute) propertySpec.Attributes[0];
            Assert.IsTrue(readOnlyAttribute.IsReadOnly);
        }

        [Test]
        public void ParameteredConstructor_FromPropertyWithAttributesWithPublicGetSet_ExpectedValues()
        {
            // Setup
            const string propertyName = "StringPropertyWithAttributes";
            PropertyInfo propertyInfo = typeof(ClassWithProperties).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            Assert.AreEqual(2, propertySpec.Attributes.Count);
            BrowsableAttribute browsableAttribute = propertySpec.Attributes.OfType<BrowsableAttribute>().Single();
            Assert.IsTrue(browsableAttribute.Browsable);
            ReadOnlyAttribute readOnlyAttribute = propertySpec.Attributes.OfType<ReadOnlyAttribute>().Single();
            Assert.IsFalse(readOnlyAttribute.IsReadOnly);
        }

        [Test]
        public void ParameteredConstructor_FromPropertyOverridingAttributesFromBaseClass_InheritedAttributesAreInherited()
        {
            // Setup
            const string propertyName = "StringPropertyWithAttributes";
            PropertyInfo propertyInfo = typeof(InheritorSettingPropertyToNotBrowsable).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            Assert.AreEqual(2, propertySpec.Attributes.Count);
            BrowsableAttribute browsableAttribute = propertySpec.Attributes.OfType<BrowsableAttribute>().Single();
            Assert.IsFalse(browsableAttribute.Browsable, "Should have the override value.");
            ReadOnlyAttribute readOnlyAttribute = propertySpec.Attributes.OfType<ReadOnlyAttribute>().Single();
            Assert.IsFalse(readOnlyAttribute.IsReadOnly, "Should have the base value.");
        }

        [Test]
        public void ParameteredConstructor_FromPropertyWithAttributesFromBaseClass_InheritedAttributesAreInherited()
        {
            // Setup
            const string propertyName = "BoolPropertyWithAttributes";
            PropertyInfo propertyInfo = typeof(InheritorSettingPropertyToNotBrowsable).GetProperty(propertyName);

            // Call
            var propertySpec = new PropertySpec(propertyInfo);

            // Assert
            Assert.AreEqual(propertyName, propertySpec.Name);
            Assert.AreEqual(propertyInfo.PropertyType.AssemblyQualifiedName, propertySpec.TypeName);
            Assert.AreEqual(1, propertySpec.Attributes.Count);
            BrowsableAttribute browsableAttribute = propertySpec.Attributes.OfType<BrowsableAttribute>().Single();
            Assert.IsTrue(browsableAttribute.Browsable,
                          "No override in 'InheritorSettingPropertyToNotBrowsable' for property 'BoolPropertyWithAttributes', so use base class.");
        }

        [Test]
        public void ParameteredConstructor_ForIndexProperty_ThrowArgumentException()
        {
            // Setup
            PropertyInfo propertyInfo = new ClassWithProperties().GetType().GetProperty("Item");

            // Call
            TestDelegate call = () => new PropertySpec(propertyInfo);

            // Assert
            const string expectedMessage = "Index properties are not allowed.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void SetValue_ProperInstanceTypeAndValueType_PropertyIsUpdated()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            propertySpec.SetValue(target, 2);

            // Assert
            Assert.AreEqual(2, target.IntegerProperty);
        }

        [Test]
        public void SetValue_IncorrectInstanceType_ThrowTargetException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            TestDelegate call = () => propertySpec.SetValue(new object(), 2);

            // Assert
            Assert.Throws<TargetException>(call);
        }

        [Test]
        public void SetValue_InstanceIsNull_ThrowTargetException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            TestDelegate call = () => propertySpec.SetValue(null, 2);

            // Assert
            Assert.Throws<TargetException>(call);
        }

        [Test]
        public void SetValue_SettingValueResultsInException_ThrowTargetInvocationException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("ThrowsException"));

            // Call
            TestDelegate call = () => propertySpec.SetValue(target, "");

            // Assert
            Exception innerException = Assert.Throws<TargetInvocationException>(call).InnerException;
            Assert.IsInstanceOf<ArgumentException>(innerException);
        }

        [Test]
        public void SetValue_PropertyWithoutPublicSet_ThrowInvalidOperationException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("DoublePropertyWithOnlyGetter"));

            // Call
            TestDelegate call = () => propertySpec.SetValue(target, 2);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            Assert.AreEqual("Property lacks public setter!", exception.Message);
        }

        [Test]
        public void SetValue_SettingValueOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            TestDelegate call = () => propertySpec.SetValue(target, new object());

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public void GetValue_ProperInstanceType_ReturnPropertyValue()
        {
            // Setup
            var target = new ClassWithProperties
            {
                IntegerProperty = 5
            };

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            object value = propertySpec.GetValue(target);

            // Assert
            Assert.AreEqual(target.IntegerProperty, value);
        }

        [Test]
        public void GetValue_PropertyHasNoPublicGetter_ThrowInvalidOperationException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("DoublePropertyWithOnlyPublicSet"));

            // Call
            TestDelegate call = () => propertySpec.GetValue(target);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            Assert.AreEqual("Property lacks public getter!", message);
        }

        [Test]
        public void GetValue_IncorrectInstanceType_ThrowArgumentException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            TestDelegate call = () => propertySpec.GetValue(new object());

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.IsInstanceOf<TargetException>(exception.InnerException);
        }

        [Test]
        public void GetValue_InstanceIsNull_ThrowArgumentException()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            TestDelegate call = () => propertySpec.GetValue(null);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.IsInstanceOf<TargetException>(exception.InnerException);
        }

        [Test]
        public void IsNonCustomExpandableObjectProperty_PropertyWithoutTypeConverter_ReturnFalse()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("IntegerProperty"));

            // Call
            bool hasExpandableObjectTypeConverter = propertySpec.IsNonCustomExpandableObjectProperty();

            // Assert
            Assert.False(hasExpandableObjectTypeConverter);
        }

        [Test]
        public void IsNonCustomExpandableObjectProperty_PropertyWithExpandableObjectTypeConverter_ReturnTrue()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("StringPropertyWithExpandableObjectConverter"));

            // Call
            bool hasExpandableObjectTypeConverter = propertySpec.IsNonCustomExpandableObjectProperty();

            // Assert
            Assert.True(hasExpandableObjectTypeConverter);
        }

        [Test]
        public void IsNonCustomExpandableObjectProperty_PropertyWithCustomExpandableObjectTypeConverter_ReturnFalse()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("StringPropertyWithCustomExpandableObjectConverter"));

            // Call
            bool hasExpandableObjectTypeConverter = propertySpec.IsNonCustomExpandableObjectProperty();

            // Assert
            Assert.False(hasExpandableObjectTypeConverter,
                         "As we cannot copy the same behavior of a ExpandableObjectConverter with customizations, we should not recognize it as such.");
        }

        [Test]
        public void IsNonCustomExpandableObjectProperty_PropertyWithSomeTypeConverter_ReturnFalse()
        {
            // Setup
            var target = new ClassWithProperties();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("StringPropertyWithSomeTypeConverter"));

            // Call
            bool hasExpandableObjectTypeConverter = propertySpec.IsNonCustomExpandableObjectProperty();

            // Assert
            Assert.False(hasExpandableObjectTypeConverter);
        }

        [Test]
        public void IsNonCustomExpandableObjectProperty_ExpandableObjectConverterInherited_ReturnTrue()
        {
            // Setup
            var target = new InheritorSettingPropertyToNotBrowsable();

            var propertySpec = new PropertySpec(target.GetType().GetProperty("StringPropertyWithExpandableObjectConverter"));

            // Call
            bool hasExpandableObjectTypeConverter = propertySpec.IsNonCustomExpandableObjectProperty();

            // Assert
            Assert.True(hasExpandableObjectTypeConverter);
        }

        private class ClassWithProperties
        {
            public float this[int index]
            {
                get
                {
                    return default(float);
                }
                set {}
            }

            public int IntegerProperty { get; set; }

            public double DoublePropertyWithOnlyPublicGet { get; private set; }

            public double DoublePropertyWithOnlyPublicSet { private get; set; }

            public double DoublePropertyWithOnlyGetter
            {
                get
                {
                    return 0.0;
                }
            }

            [Browsable(true)]
            [ReadOnly(false)]
            public virtual string StringPropertyWithAttributes { get; set; }

            [Browsable(true)]
            public bool BoolPropertyWithAttributes { get; set; }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public virtual string StringPropertyWithExpandableObjectConverter { get; set; }

            [TypeConverter(typeof(CustomExpandableObjectConverter))]
            public string StringPropertyWithCustomExpandableObjectConverter { get; set; }

            [TypeConverter(typeof(SomeTypeConverter))]
            public string StringPropertyWithSomeTypeConverter { get; set; }

            public string ThrowsException
            {
                set
                {
                    throw new ArgumentException();
                }
            }
        }

        private class InheritorSettingPropertyToNotBrowsable : ClassWithProperties
        {
            [Browsable(false)]
            public override string StringPropertyWithAttributes { get; set; }

            public override string StringPropertyWithExpandableObjectConverter { get; set; }
        }

        private class SomeTypeConverter : TypeConverter {}

        private class CustomExpandableObjectConverter : ExpandableObjectConverter {}
    }
}