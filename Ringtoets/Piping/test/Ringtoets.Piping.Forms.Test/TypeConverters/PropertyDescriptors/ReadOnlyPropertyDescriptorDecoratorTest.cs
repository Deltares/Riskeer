using System.ComponentModel;

using NUnit.Framework;

using Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors;

namespace Ringtoets.Piping.Forms.Test.TypeConverters.PropertyDescriptors
{
    [TestFixture]
    public class ReadOnlyPropertyDescriptorDecoratorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var target = new SomeTestClass();
            var properties = TypeDescriptor.GetProperties(target);

            PropertyDescriptor getSetProperty = properties[0];

            // Precondtion:
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
            var properties = TypeDescriptor.GetProperties(component);

            PropertyDescriptor getSetProperty = properties[0];
            var wrappedProperty = new ReadOnlyPropertyDescriptorDecorator(getSetProperty);

            // Call
            var result = wrappedProperty.CanResetValue(component);

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

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            var wrappedProperty = new ReadOnlyPropertyDescriptorDecorator(getSetProperty);

            // Call
            var result = wrappedProperty.GetValue(component);

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

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            getSetProperty.ResetValue(component);
            var expectedPropertyValueAfterReset = component.SomeEditableProperty;

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

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor getSetProperty = properties[0];

            getSetProperty.SetValue(component, newValue);
            var expectedPropertyValueAfterSet = component.SomeEditableProperty;

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
            var properties = TypeDescriptor.GetProperties(component);

            PropertyDescriptor getSetProperty = properties[0];
            var wrappedProperty = new ReadOnlyPropertyDescriptorDecorator(getSetProperty);

            // Call
            var result = wrappedProperty.ShouldSerializeValue(component);

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