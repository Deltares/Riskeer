using System.ComponentModel;
using NUnit.Framework;
using Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors;

namespace Ringtoets.Piping.Forms.Test.TypeConverters.PropertyDescriptors
{
    [TestFixture]
    public class ContainingPropertyUpdateDescriptorDecoratorTest
    {
        [Test]
        public void ParameteredConstructor_ExpecedValues()
        {
            // Setup
            var a = new TestA();
            var propertyDescription = TypeDescriptor.GetProperties(a.Child)[0];

            // Call
            var routedPropertyDescriptor = new ContainingPropertyUpdateDescriptorDecorator(propertyDescription, a, TypeDescriptor.GetProperties(a)[0]);
            // Assert
            Assert.AreEqual(propertyDescription.ComponentType, routedPropertyDescriptor.ComponentType);
            Assert.AreEqual(propertyDescription.IsReadOnly, routedPropertyDescriptor.IsReadOnly);
            Assert.AreEqual(propertyDescription.PropertyType, routedPropertyDescriptor.PropertyType);

            Assert.AreEqual(propertyDescription.Name, routedPropertyDescriptor.Name);
            Assert.AreEqual(propertyDescription.Attributes, routedPropertyDescriptor.Attributes);
            Assert.AreEqual(propertyDescription.Category, routedPropertyDescriptor.Category);
            Assert.AreEqual(propertyDescription.Converter, routedPropertyDescriptor.Converter);
            Assert.AreEqual(propertyDescription.Description, routedPropertyDescriptor.Description);
            Assert.AreEqual(propertyDescription.DesignTimeOnly, routedPropertyDescriptor.DesignTimeOnly);
            Assert.AreEqual(propertyDescription.DisplayName, routedPropertyDescriptor.DisplayName);
            Assert.AreEqual(propertyDescription.IsBrowsable, routedPropertyDescriptor.IsBrowsable);
            Assert.AreEqual(propertyDescription.IsLocalizable, routedPropertyDescriptor.IsLocalizable);
            Assert.AreEqual(propertyDescription.SerializationVisibility, routedPropertyDescriptor.SerializationVisibility);
            Assert.AreEqual(propertyDescription.SupportsChangeEvents, routedPropertyDescriptor.SupportsChangeEvents);
        }

        [Test]
        public void CanResetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            var component = new TestB();
            var properties = TypeDescriptor.GetProperties(component);

            PropertyDescriptor getSetProperty = properties[0];
            var wrappedProperty = new ContainingPropertyUpdateDescriptorDecorator(getSetProperty, null, null);

            // Call
            var result = wrappedProperty.CanResetValue(component);

            // Assert
            Assert.AreEqual(getSetProperty.CanResetValue(component), result);
        }

        [Test]
        public void GetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            var component = new TestB
            {
                Value = 1.1
            };

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor valueProperty = properties[0];

            var wrappedProperty = new ContainingPropertyUpdateDescriptorDecorator(valueProperty, null, null);

            // Call
            var result = wrappedProperty.GetValue(component);

            // Assert
            Assert.AreEqual(valueProperty.GetValue(component), result);
            Assert.AreEqual(component.Value, result);
        }

        [Test]
        public void ResetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            var component = new TestB
            {
                Value = originalPropertyValue
            };

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor valueProperty = properties[0];

            valueProperty.ResetValue(component);
            var expectedPropertyValueAfterReset = component.Value;

            var wrappedProperty = new ContainingPropertyUpdateDescriptorDecorator(valueProperty, null, null);
            component.Value = originalPropertyValue;

            // Call
            wrappedProperty.ResetValue(component);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterReset, component.Value);
        }

        [Test]
        public void SetValue_Always_DelegateToWrappedPropertyDescriptor()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            const double newValue = 2.2;
            var component = new TestB
            {
                Value = originalPropertyValue
            };

            var properties = TypeDescriptor.GetProperties(component);
            PropertyDescriptor valueProperty = properties[0];

            valueProperty.SetValue(component, newValue);
            var expectedPropertyValueAfterReset = component.Value;

            var wrappedProperty = new ContainingPropertyUpdateDescriptorDecorator(valueProperty, null, null);
            component.Value = originalPropertyValue;

            // Call
            wrappedProperty.SetValue(component, newValue);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterReset, component.Value);
        }

        [Test]
        public void SetValue_WithParentAndParentProperty_CallSetterOfParentForTheProperty()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            const double newValue = 2.2;
            var a = new TestA
            {
                Child =
                {
                    Value = originalPropertyValue
                }
            };

            PropertyDescriptor getSetProperty = TypeDescriptor.GetProperties(a.Child)[0]; // Value property
            var sourcePropertySetDescriptorDecorator = new ContainingPropertyUpdateDescriptorDecorator(getSetProperty, a, TypeDescriptor.GetProperties(a)[0]);
            a.Child.Value = originalPropertyValue;

            // Call
            sourcePropertySetDescriptorDecorator.SetValue(a.Child, newValue);

            // Assert
            Assert.AreEqual(2, a.SetterHit);
        }

        [Test]
        public void SetValue_WithoutParent_NoCallSetterOfParentForTheProperty()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            const double newValue = 2.2;
            var a = new TestA
            {
                Child =
                {
                    Value = originalPropertyValue
                }
            };

            PropertyDescriptor getSetProperty = TypeDescriptor.GetProperties(a.Child)[0]; // Value property
            var sourcePropertySetDescriptorDecorator = new ContainingPropertyUpdateDescriptorDecorator(getSetProperty, null, TypeDescriptor.GetProperties(a)[0]);
            a.Child.Value = originalPropertyValue;

            // Call
            sourcePropertySetDescriptorDecorator.SetValue(a.Child, newValue);

            // Assert
            Assert.AreEqual(1, a.SetterHit);
        }

        [Test]
        public void SetValue_WithoutParentProperty_NoCallSetterOfParentForTheProperty()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            const double newValue = 2.2;
            var a = new TestA
            {
                Child =
                {
                    Value = originalPropertyValue
                }
            };

            PropertyDescriptor getSetProperty = TypeDescriptor.GetProperties(a.Child)[0]; // Value property
            var sourcePropertySetDescriptorDecorator = new ContainingPropertyUpdateDescriptorDecorator(getSetProperty, a, null);
            a.Child.Value = originalPropertyValue;

            // Call
            sourcePropertySetDescriptorDecorator.SetValue(a.Child, newValue);

            // Assert
            Assert.AreEqual(1, a.SetterHit);
        }

        private class TestA
        {
            private TestB child;

            public TestA()
            {
                Child = new TestB();
            }

            public TestB Child
            {
                get
                {
                    return child;
                }
                set
                {
                    child = value;
                    SetterHit++;
                }
            }

            public int SetterHit { get; private set; }
        }

        private class TestB
        {
            public double Value { get; set; }
        }
    }
}