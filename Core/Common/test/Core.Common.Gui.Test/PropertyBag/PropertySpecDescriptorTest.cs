// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Reflection;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;

namespace Core.Common.Gui.Test.PropertyBag
{
    [TestFixture]
    public class PropertySpecDescriptorTest
    {
        [Test]
        public void Constructor_PropertySpecNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PropertySpecDescriptor(null, new object());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertySpec", paramName);
        }

        [Test]
        public void ParameteredConstructor_IsPropertyReadOnlyProperty_ExpectedValues()
        {
            // Setup
            var instance = new ClassWithProperties();
            PropertyInfo propertyInfo = instance.GetType().GetProperty("IsPropertyReadOnly");
            var spec = new PropertySpec(propertyInfo);

            // Call
            var propertyDescriptor = new PropertySpecDescriptor(spec, instance);

            // Assert
            Assert.AreEqual(spec.GetType(), propertyDescriptor.ComponentType);
            Assert.IsFalse(propertyDescriptor.IsReadOnly);
            Assert.IsTrue(propertyDescriptor.IsBrowsable);
            Assert.AreEqual(propertyInfo.PropertyType, propertyDescriptor.PropertyType);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsReadOnly_PropertyHasDynamicReadOnlyProperty_ReturnExpectedValue(bool isPropertyReadOnly)
        {
            // Setup
            var instance = new ClassWithProperties
            {
                IsPropertyReadOnly = isPropertyReadOnly
            };
            var propertySpec = new PropertySpec(instance.GetType().GetProperty("IntegerPropertyWithDynamicReadOnly"));
            var descriptor = new PropertySpecDescriptor(propertySpec, instance);

            // Call
            bool isReadOnly = descriptor.IsReadOnly;

            // Assert
            Assert.AreEqual(isPropertyReadOnly, isReadOnly);
        }

        [Test]
        public void IsReadOnly_PropertyHasReadOnlyTrueAttribute_ReturnTrue()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("PropertyWithReadOnlyAttribute"));
            var descriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            bool isReadOnly = descriptor.IsReadOnly;

            // Assert
            Assert.IsTrue(isReadOnly);
        }

        [Test]
        public void IsReadOnly_PropertyHasReadOnlyFalseAttribute_ReturnFalse()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("PropertyWithReadOnlyFalseAttribute"));
            var descriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            bool isReadOnly = descriptor.IsReadOnly;

            // Assert
            Assert.IsFalse(isReadOnly);
        }

        [Test]
        public void IsReadOnly_PropertyHasNoAttributeAndOnlyGetter_ReturnTrue()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("PropertyWithOnlyGetter"));
            var descriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            bool isReadOnly = descriptor.IsReadOnly;

            // Assert
            Assert.IsTrue(isReadOnly);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsBrowsable_PropertyHasDynamicBrowsableProperty_ReturnExpectedValue(bool isPropertyVisible)
        {
            // Setup
            var instance = new ClassWithProperties
            {
                IsPropertyBrowsable = isPropertyVisible
            };
            var propertySpec = new PropertySpec(instance.GetType().GetProperty("IntegerPropertyWithDynamicVisibility"));
            var descriptor = new PropertySpecDescriptor(propertySpec, instance);

            // Call
            bool isBrowsable = descriptor.IsBrowsable;

            // Assert
            Assert.AreEqual(isPropertyVisible, isBrowsable);
        }

        [Test]
        public void IsIsBrowsable_PropertyHasBrowsableTrueAttribute_ReturnTrue()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("PropertyWithBrowsableAttribute"));
            var descriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            bool isBrowsable = descriptor.IsBrowsable;

            // Assert
            Assert.IsTrue(isBrowsable);
        }

        [Test]
        public void IsBrowsable_PropertyHasBrowsableFalseAttribute_ReturnFalse()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("PropertyWithBrowsableFalseAttribute"));
            var descriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            bool isBrowsable = descriptor.IsBrowsable;

            // Assert
            Assert.IsFalse(isBrowsable);
        }

        [Test]
        public void CanResetValue_ReturnFalse()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("PropertyWithOnlyGetter"));
            var propertyDescriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            bool canReset = propertyDescriptor.CanResetValue(instance);

            // Assert
            Assert.IsFalse(canReset);
        }

        [Test]
        public void ResetValue_CanNotReset_ThrowsInvalidOperationException()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("PropertyWithOnlyGetter"));
            var propertyDescriptor = new PropertySpecDescriptor(spec, instance);

            // Precondition
            Assert.IsFalse(propertyDescriptor.CanResetValue(instance));

            // Call
            TestDelegate call = () => propertyDescriptor.ResetValue(instance);

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void ShouldSerializeValue_ReturnFalse()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("PropertyWithOnlyGetter"));
            var propertyDescriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            bool shouldSerializeValue = propertyDescriptor.ShouldSerializeValue(instance);

            // Assert
            Assert.IsFalse(shouldSerializeValue);
        }

        [Test]
        public void GetValue_SimpleValueProperty_ReturnPropertyValue()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("PropertyWithOnlyGetter"));
            var propertyDescriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            object value = propertyDescriptor.GetValue(instance);

            // Assert
            Assert.AreEqual(instance.PropertyWithOnlyGetter, value);
        }

        [Test]
        public void GetValue_ObjectValueProperty_ReturnPropertyValue()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("ComplexSubProperty"));
            var propertyDescriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            object value = propertyDescriptor.GetValue(instance);

            // Assert
            Assert.AreSame(instance.ComplexSubProperty, value);
        }

        [Test]
        public void GetValue_ObjectValuePropertyWithExpandableObjectConverterAttribute_ReturnPropertyValueWrappedInDynamicPropertyBag()
        {
            // Setup
            var instance = new ClassWithProperties();
            var spec = new PropertySpec(instance.GetType().GetProperty("ComplexSubPropertyWithExandableObjectConverter"));
            var propertyDescriptor = new PropertySpecDescriptor(spec, instance);

            // Call
            object value = propertyDescriptor.GetValue(instance);

            // Assert
            var dynamicPropertyBag = (DynamicPropertyBag) value;
            Assert.IsNotNull(dynamicPropertyBag);
            Assert.AreSame(instance.ComplexSubPropertyWithExandableObjectConverter, dynamicPropertyBag.WrappedObject);
        }

        private class ClassWithProperties
        {
            public ClassWithProperties()
            {
                IsPropertyReadOnly = false;
                IsPropertyBrowsable = true;
                ComplexSubPropertyWithExandableObjectConverter = new AnotherClassWithProperties
                {
                    Comment = "I have nice type converter, right?"
                };
                ComplexSubProperty = new AnotherClassWithProperties
                {
                    Comment = "Don't want your type converter!"
                };
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public AnotherClassWithProperties ComplexSubPropertyWithExandableObjectConverter { get; }

            public AnotherClassWithProperties ComplexSubProperty { get; }

            #region IsReadOnly state influencing testing members

            public string PropertyWithOnlyGetter
            {
                get
                {
                    return "I only have a getter.";
                }
            }

            public bool IsPropertyReadOnly { get; set; }

            [ReadOnly(true)]
            public double PropertyWithReadOnlyAttribute { get; set; }

            [ReadOnly(false)]
            public double PropertyWithReadOnlyFalseAttribute { get; set; }

            [DynamicReadOnly]
            public int IntegerPropertyWithDynamicReadOnly { get; set; }

            [DynamicReadOnlyValidationMethod]
            public bool IsReadOnly(string propertyName)
            {
                if (propertyName == "IntegerPropertyWithDynamicReadOnly")
                {
                    return IsPropertyReadOnly;
                }

                throw new NotImplementedException();
            }

            #endregion

            #region IsBrowsable state influencing testing members

            public bool IsPropertyBrowsable { get; set; }

            [Browsable(true)]
            public double PropertyWithBrowsableAttribute { get; set; }

            [Browsable(false)]
            public double PropertyWithBrowsableFalseAttribute { get; set; }

            [DynamicVisible]
            public int IntegerPropertyWithDynamicVisibility { get; set; }

            [DynamicVisibleValidationMethod]
            public bool IsVisible(string propertyName)
            {
                if (propertyName == "IntegerPropertyWithDynamicVisibility")
                {
                    return IsPropertyBrowsable;
                }

                throw new NotImplementedException();
            }

            #endregion
        }

        private class AnotherClassWithProperties
        {
            public string Comment { get; set; }
        }
    }
}