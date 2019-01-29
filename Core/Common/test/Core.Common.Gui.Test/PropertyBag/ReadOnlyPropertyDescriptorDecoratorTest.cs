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

using System.ComponentModel;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;

namespace Core.Common.Gui.Test.PropertyBag
{
    [TestFixture]
    public class ReadOnlyPropertyDescriptorDecoratorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var target = new SomeTestClass();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(target);

            PropertyDescriptor getSetProperty = properties[0];

            // Precondition:
            Assert.IsFalse(getSetProperty.IsReadOnly);

            // Call
            var readonlyPropertyDescriptor = new ReadOnlyPropertyDescriptorDecorator(getSetProperty);

            // Assert
            Assert.IsInstanceOf<PropertyDescriptor>(readonlyPropertyDescriptor);
            Assert.IsTrue(readonlyPropertyDescriptor.IsReadOnly);

            Assert.AreEqual(getSetProperty.ComponentType, readonlyPropertyDescriptor.ComponentType);
            Assert.AreEqual(getSetProperty.PropertyType, readonlyPropertyDescriptor.PropertyType);
            Assert.AreEqual(getSetProperty.Attributes, readonlyPropertyDescriptor.Attributes);
            Assert.AreEqual(getSetProperty.Category, readonlyPropertyDescriptor.Category);
            Assert.AreEqual(getSetProperty.Converter, readonlyPropertyDescriptor.Converter);
            Assert.AreEqual(getSetProperty.Description, readonlyPropertyDescriptor.Description);
            Assert.AreEqual(getSetProperty.DesignTimeOnly, readonlyPropertyDescriptor.DesignTimeOnly);
            Assert.AreEqual(getSetProperty.DisplayName, readonlyPropertyDescriptor.DisplayName);
            Assert.AreEqual(getSetProperty.IsBrowsable, readonlyPropertyDescriptor.IsBrowsable);
            Assert.AreEqual(getSetProperty.IsLocalizable, readonlyPropertyDescriptor.IsLocalizable);
            Assert.AreEqual(getSetProperty.Name, readonlyPropertyDescriptor.Name);
            Assert.AreEqual(getSetProperty.SerializationVisibility, readonlyPropertyDescriptor.SerializationVisibility);
            Assert.AreEqual(getSetProperty.SupportsChangeEvents, readonlyPropertyDescriptor.SupportsChangeEvents);
        }

        [Test]
        public void CanResetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            var component = new SomeTestClass();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);

            PropertyDescriptor getSetProperty = properties[0];
            var wrappedProperty = new ReadOnlyPropertyDescriptorDecorator(getSetProperty);

            // Call
            bool result = wrappedProperty.CanResetValue(component);

            // Assert
            Assert.AreEqual(getSetProperty.CanResetValue(component), result);
        }

        [Test]
        public void GetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            var component = new SomeTestClass
            {
                SomeEditableProperty = 1.1
            };

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            var wrappedProperty = new ReadOnlyPropertyDescriptorDecorator(getSetProperty);

            // Call
            object result = wrappedProperty.GetValue(component);

            // Assert
            Assert.AreEqual(getSetProperty.GetValue(component), result);
            Assert.AreEqual(component.SomeEditableProperty, result);
        }

        [Test]
        public void ResetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            var component = new SomeTestClass
            {
                SomeEditableProperty = originalPropertyValue
            };

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            getSetProperty.ResetValue(component);
            double expectedPropertyValueAfterReset = component.SomeEditableProperty;

            var wrappedProperty = new ReadOnlyPropertyDescriptorDecorator(getSetProperty);
            component.SomeEditableProperty = originalPropertyValue;

            // Call
            wrappedProperty.ResetValue(component);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterReset, component.SomeEditableProperty);
        }

        [Test]
        public void SetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            const double newValue = 2.2;
            var component = new SomeTestClass
            {
                SomeEditableProperty = originalPropertyValue
            };

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            getSetProperty.SetValue(component, newValue);
            double expectedPropertyValueAfterSet = component.SomeEditableProperty;

            var wrappedProperty = new ReadOnlyPropertyDescriptorDecorator(getSetProperty);
            component.SomeEditableProperty = originalPropertyValue;

            // Call
            wrappedProperty.SetValue(component, newValue);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterSet, component.SomeEditableProperty);
        }

        [Test]
        public void ShouldSerializeValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            var component = new SomeTestClass();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);

            PropertyDescriptor getSetProperty = properties[0];
            var wrappedProperty = new ReadOnlyPropertyDescriptorDecorator(getSetProperty);

            // Call
            bool result = wrappedProperty.ShouldSerializeValue(component);

            // Assert
            Assert.AreEqual(getSetProperty.ShouldSerializeValue(component), result);
        }

        private class SomeTestClass
        {
            [ReadOnly(false)]
            public double SomeEditableProperty { get; set; }
        }
    }
}