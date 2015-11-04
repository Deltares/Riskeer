using System.ComponentModel;

using NUnit.Framework;

using Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors;

namespace Ringtoets.Piping.Forms.Test.TypeConverters.PropertyDescriptors
{
    [TestFixture]
    public class RoutedPropertyDescriptorTest
    {
        [Test]
        public void ParameteredConstructpr_ExpecedValues()
        {
            // Setup
            var a = new TestA();
            var propertyDescription = TypeDescriptor.GetProperties(a.Child)[0];

            // Call
            var routedPropertyDescriptor = new RoutedPropertyDescriptor(propertyDescription, o => ((TestA)o).Child);

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
        public void CanResetValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
        {
            // Setup
            var a = new TestA();
            var propertyDescription = TypeDescriptor.GetProperties(a.Child)[0];

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(propertyDescription, o => ((TestA)o).Child);

            // Call
            var result = routedPropertyDescriptor.CanResetValue(a);

            // Assert
            Assert.AreEqual(propertyDescription.CanResetValue(a.Child), result);
        }

        [Test]
        public void GetValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
        {
            // Setup
            var a = new TestA();
            var propertyDescription = TypeDescriptor.GetProperties(a.Child)[0];

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(propertyDescription, o => ((TestA)o).Child);

            // Call
            var result = routedPropertyDescriptor.GetValue(a);

            // Assert
            Assert.AreEqual(propertyDescription.GetValue(a.Child), result);
        }

        [Test]
        public void ResetValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            var a = new TestA
            {
                Child =
                {
                    Value = originalPropertyValue
                }
            };

            PropertyDescriptor getSetProperty = TypeDescriptor.GetProperties(a.Child)[0];

            getSetProperty.ResetValue(a.Child);
            var expectedPropertyValueAfterReset = a.Child.Value;

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(getSetProperty, o => ((TestA)o).Child);
            a.Child.Value = originalPropertyValue;

            // Call
            routedPropertyDescriptor.ResetValue(a);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterReset, a.Child.Value);
        }

        [Test]
        public void SetValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
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

            PropertyDescriptor getSetProperty = TypeDescriptor.GetProperties(a.Child)[0];

            getSetProperty.SetValue(a.Child, newValue);
            var expectedPropertyValueAfterReset = a.Child.Value;

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(getSetProperty, o => ((TestA)o).Child);
            a.Child.Value = originalPropertyValue;

            // Call
            routedPropertyDescriptor.SetValue(a, newValue);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterReset, a.Child.Value);
        }

        [Test]
        public void ShouldSerializeValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
        {
            // Setup
            var a = new TestA();
            var propertyDescription = TypeDescriptor.GetProperties(a.Child)[0];

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(propertyDescription, o => ((TestA)o).Child);

            // Call
            var result = routedPropertyDescriptor.ShouldSerializeValue(a);

            // Assert
            Assert.AreEqual(propertyDescription.ShouldSerializeValue(a.Child), result);
        }

        private class TestA
        {
            public TestA()
            {
                Child = new TestB();
            }

            public TestB Child { get; set; }
        }

        private class TestB
        {
            public double Value { get; set; }
        }
    }
}