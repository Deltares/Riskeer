// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.ComponentModel;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors;

namespace Ringtoets.Piping.Forms.Test.TypeConverters.PropertyDescriptors
{
    [TestFixture]
    public class TextPropertyDescriptorDecoratorTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            const string displayName = "<My custom display name>";
            const string description = "<My custom description>";

            var target = new ClassWithDoubleProperty();
            var properties = TypeDescriptor.GetProperties(target);

            PropertyDescriptor getSetProperty = properties[0];

            // Call
            var wrappedProperty = new TextPropertyDescriptorDecorator(getSetProperty, displayName, description);

            // Assert
            Assert.AreEqual(displayName, wrappedProperty.DisplayName);
            Assert.AreEqual(description, wrappedProperty.Description);

            Assert.AreEqual(getSetProperty.ComponentType, wrappedProperty.ComponentType);
            Assert.AreEqual(getSetProperty.IsReadOnly, wrappedProperty.IsReadOnly);
            Assert.AreEqual(getSetProperty.PropertyType, wrappedProperty.PropertyType);
            Assert.AreEqual(getSetProperty.Name, wrappedProperty.Name);
            Assert.AreEqual(getSetProperty.Attributes, wrappedProperty.Attributes);
            Assert.AreEqual(getSetProperty.Category, wrappedProperty.Category);
            Assert.AreEqual(getSetProperty.Converter, wrappedProperty.Converter);
            Assert.AreEqual(getSetProperty.DesignTimeOnly, wrappedProperty.DesignTimeOnly);
            Assert.AreEqual(getSetProperty.IsBrowsable, wrappedProperty.IsBrowsable);
            Assert.AreEqual(getSetProperty.IsLocalizable, wrappedProperty.IsLocalizable);
            Assert.AreEqual(getSetProperty.SerializationVisibility, wrappedProperty.SerializationVisibility);
            Assert.AreEqual(getSetProperty.SupportsChangeEvents, wrappedProperty.SupportsChangeEvents);

            Assert.IsNull(wrappedProperty.ObservableParent);
        }

        [Test]
        public void CanResetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            var component = new ClassWithDoubleProperty();
            var properties = TypeDescriptor.GetProperties(component);

            PropertyDescriptor getSetProperty = properties[0];
            var wrappedProperty = new TextPropertyDescriptorDecorator(getSetProperty, null, null);

            // Call
            var result = wrappedProperty.CanResetValue(component);

            // Assert
            Assert.AreEqual(getSetProperty.CanResetValue(component), result);
        }

        [Test]
        public void GetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            var component = new ClassWithDoubleProperty
            {
                GetSetProperty = 1.1
            };

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            var wrappedProperty = new TextPropertyDescriptorDecorator(getSetProperty, null, null);

            // Call
            var result = wrappedProperty.GetValue(component);

            // Assert
            Assert.AreEqual(getSetProperty.GetValue(component), result);
            Assert.AreEqual(component.GetSetProperty, result);
        }

        [Test]
        public void ResetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            var component = new ClassWithDoubleProperty
            {
                GetSetProperty = originalPropertyValue
            };

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            getSetProperty.ResetValue(component);
            var expectedPropertyValueAfterReset = component.GetSetProperty;

            var wrappedProperty = new TextPropertyDescriptorDecorator(getSetProperty, null, null);
            component.GetSetProperty = originalPropertyValue;

            // Call
            wrappedProperty.ResetValue(component);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterReset, component.GetSetProperty);
        }

        [Test]
        public void SetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            const double newValue = 2.2;
            var component = new ClassWithDoubleProperty
            {
                GetSetProperty = originalPropertyValue
            };

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            getSetProperty.SetValue(component, newValue);
            var expectedPropertyValueAfterReset = component.GetSetProperty;

            var wrappedProperty = new TextPropertyDescriptorDecorator(getSetProperty, null, null);
            component.GetSetProperty = originalPropertyValue;

            // Call
            wrappedProperty.SetValue(component, newValue);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterReset, component.GetSetProperty);
        }

        [Test]
        public void SetValue_ObservableParentProvided_NotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            const double originalPropertyValue = 1.1;
            const double newValue = 2.2;
            var component = new ClassWithDoubleProperty
            {
                GetSetProperty = originalPropertyValue
            };

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            var wrappedProperty = new TextPropertyDescriptorDecorator(getSetProperty, null, null)
            {
                ObservableParent = observable
            };

            // Call
            wrappedProperty.SetValue(component, newValue);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ShouldSerializeValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            var component = new ClassWithDoubleProperty();
            var properties = TypeDescriptor.GetProperties(component);

            PropertyDescriptor getSetProperty = properties[0];
            var wrappedProperty = new TextPropertyDescriptorDecorator(getSetProperty, null, null);

            // Call
            var result = wrappedProperty.ShouldSerializeValue(component);

            // Assert
            Assert.AreEqual(getSetProperty.ShouldSerializeValue(component), result);
        }

        private class ClassWithDoubleProperty
        {
            public double GetSetProperty { get; set; }
        }
    }
}